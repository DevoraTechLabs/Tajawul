from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import json
import faiss
import numpy as np
import re
import logging
import requests
from sentence_transformers import SentenceTransformer
from transformers import AutoModelForCausalLM, AutoTokenizer, pipeline,BitsAndBytesConfig
from langdetect import detect
from langdetect.lang_detect_exception import LangDetectException
from typing import Dict, List, Tuple, Optional
from fastapi.responses import JSONResponse
import warnings
import os
import time
import threading
from huggingface_hub import HfApi, HfFolder
app = FastAPI()
    
@app.get("/")
def root():
    return JSONResponse({"message": "FastAPI is running in Colab!"})
warnings.filterwarnings('ignore')

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

CONFIG_FILE = "config.json"

def load_config():
    if not os.path.exists(CONFIG_FILE):
        return {"hugging_face_key": "", "json_path": "DATA_RQ.json"}
    with open(CONFIG_FILE, "r") as f:
        return json.load(f)

def save_config(config):
    with open(CONFIG_FILE, "w") as f:
        json.dump(config, f, indent=4)

config = load_config()
HUGGING_FACE_KEY = config.get("hugging_face_key", "")
def login_hugging_face(api_key: str):
    try:
        # Set the Hugging Face key as an environment variable
        os.environ["HF_API_KEY"] = api_key
        # Authenticate the user using the API key
        HfFolder.save_token(api_key)
        hf_api = HfApi()
        user_info = hf_api.whoami()  # Get user info to confirm login
        return {"message": f"Logged in successfully as {user_info['name']}"}
    except Exception as e:
        return {"error": f"Failed to log in: {str(e)}"}
login_hugging_face(HUGGING_FACE_KEY)
class DataLoader:
    def __init__(self, json_path: str):
        self.json_path = json_path
        self.qa_pairs: List[Tuple[str, str]] = []
        self.instructions: List[str] = []

    def load(self) -> Tuple[List[Tuple[str, str]], List[str]]:
        try:
            with open(self.json_path, "r", encoding="utf-8") as f:
                data = json.load(f)
            self._parse_json(data)
            return self.qa_pairs, self.instructions
        except (FileNotFoundError, json.JSONDecodeError) as e:
            logger.error(f"Error loading JSON data: {e}")
            raise

    def _parse_json(self, data: dict) -> None:
        for category, content in data.items():
            try:
                if isinstance(content, list):
                    for entry in content:
                        if isinstance(entry, dict):
                            question = entry.get("Q") or entry.get("question")
                            answer = entry.get("A") or entry.get("answer")
                            if isinstance(answer, list):
                                answer = " ".join(answer)
                            if question and answer:
                                self.qa_pairs.append((question, answer))
                        elif isinstance(entry, list):
                            for sub_entry in entry:
                                question = sub_entry.get("question")
                                answer = sub_entry.get("answer")
                                if isinstance(answer, list):
                                    answer = " ".join(answer)
                                if question and answer:
                                    self.qa_pairs.append((question, answer))
                        else:
                            self.instructions.append(str(entry))
                elif isinstance(content, dict):
                    for value in content.values():
                        if isinstance(value, dict):
                            question = value.get("question")
                            answer = value.get("answer")
                            if isinstance(answer, list):
                                answer = " ".join(answer)
                            if question and answer:
                                self.qa_pairs.append((question, answer))
                        elif isinstance(value, list):
                            self.instructions.extend(value)
                        else:
                            self.instructions.append(str(value))
            except Exception as e:
                logger.warning(f"Error processing category '{category}': {e}")

        logger.info(f"Loaded {len(self.qa_pairs)} Q&A pairs and {len(self.instructions)} instructions.")


class EmbeddingPipeline:
    def __init__(self, qa_pairs: List[Tuple[str, str]], instructions: List[str]):
        self.qa_pairs = qa_pairs
        self.instructions = instructions
        self.embedder = SentenceTransformer("sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2")
        self.index = None
        self.all_data: List[Tuple[str, str]] = []

    def build_index(self) -> None:
        question_embeddings = self.embedder.encode([q[0] for q in self.qa_pairs])
        instruction_embeddings = self.embedder.encode(self.instructions)

        dimension = question_embeddings.shape[1]
        self.index = faiss.IndexFlatL2(dimension)
        self.index.add(np.array(question_embeddings))
        self.index.add(np.array(instruction_embeddings))
        self.all_data = self.qa_pairs + [(inst, inst) for inst in self.instructions]
        logger.info(f"FAISS index built with {len(self.all_data)} entries.")

    def retrieve(self, query: str, top_k: int = 3, threshold: float = 0.7) -> List[str]:
        try:
            query_embedding = self.embedder.encode([query])
            distances, indices = self.index.search(np.array(query_embedding), top_k)
            return [self.all_data[idx][1] for dist, idx in zip(distances[0], indices[0]) if dist < threshold]
        except Exception as e:
            logger.error(f"Error retrieving answer: {e}")
            return []


class TextGenerator:
    def __init__(self):
        self.tokenizer = AutoTokenizer.from_pretrained("CohereForAI/aya-expanse-8b", timeout=600)
        self.quantization_config =BitsAndBytesConfig(load_in_4bit=True, bnb_4bit_quant_type="nf4", bnb_4bit_use_double_quant=True, bnb_4bit_compute_dtype="float16" )
        self.model = AutoModelForCausalLM.from_pretrained("CohereForAI/aya-expanse-8b", quantization_config=self.quantization_config ,device_map="auto")
        self.pipeline = pipeline("text-generation", model=self.model, tokenizer=self.tokenizer)

    def generate(self, prompt: str, min_length: int, max_length: int) -> str:
        try:
            outputs = self.pipeline(
                prompt,
                max_length=max_length,
                min_length=min_length,
                do_sample=True,
                top_p=0.85,
                temperature=0.7,
                eos_token_id=self.tokenizer.eos_token_id
            )
            return outputs[0]['generated_text'].replace(prompt, "").strip()
        except Exception as e:
            logger.error(f"Error generating text: {e}")
            return ""


class ChatbotPipeline:
    def __init__(self, json_path: str):
        data_loader = DataLoader(json_path)
        qa_pairs, instructions = data_loader.load()

        self.user_conversations: Dict[str, List[str]] = {}
        self.embedding = EmbeddingPipeline(qa_pairs, instructions)
        self.embedding.build_index()
        self.generator = TextGenerator()

       
        self.idle_thread = threading.Thread(target=self._idle_behavior)
        self.idle_thread.daemon = True
        self.idle_thread.start()

    def _idle_behavior(self):
        while True:
            time.sleep(15 * 60)  
            logger.info("No new request for 15 minutes. Sleeping.")

    def fetch_user_info(self, token: str) -> Optional[Dict]:
        url = "https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/User/info"
        headers = {"Authorization": f"Bearer {token}"}
        try:
            response = requests.get(url, headers=headers)
            response.raise_for_status()
            data = response.json()

            user_info = {
                "firstName": data.get("firstName"),
                "lastName": data.get("lastName"),
                "tags": data.get("tags", []),
                "spokenLanguages": data.get("spokenLanguages", []),
                "lastMessages": data.get("lastMessages", [])[-5:]
            }
            logger.info(f"Fetched user info: {user_info}")
            return user_info
        except requests.exceptions.RequestException as e:
            logger.error(f"Error fetching user info: {e}")
            return None

    def get_dynamic_word_range(self, user_input: str, retrieved_answers: List[str]) -> Tuple[int, int]:
        try:
            language = detect(user_input)
        except:
            language = "en"

        num_retrieved = sum(len(ans.split()) for ans in retrieved_answers)
        avg_len = num_retrieved + len(user_input.split())

        language_factors = {
            "en": 1.0, "ar": 0.8, "fr": 1.1, "zh": 0.5, "es": 1.05,
            "de": 1.0, "ru": 1.1, "it": 1.0, "pt": 1.05, "ja": 0.6, "ko": 0.7, "tr": 0.9
        }
        factor = language_factors.get(language, 1.0)

        base_max_len = int((avg_len + 30) * factor)
        return max(150, base_max_len - 100), max(500, base_max_len + 200)

    def format_response(self, text: str) -> str:
        text = re.sub(r"<\|im_end\|>|<\|endoftext\|>", "", text).strip()
        text = re.sub(r"\n", "", text).strip()
        paragraphs = text.split("\n")
        lines = []
        for paragraph in paragraphs:
            line = ""
            for word in paragraph.split():
                if len(line) + len(word) > 90:
                    lines.append(line.strip())
                    line = ""
                line += f"{word} "
            if line:
                lines.append(line.strip())
        return "\n".join(lines)

    def add_bullets(self, text: str) -> str:
        lines = text.split("\n")
        return "\n".join([f"- {line.strip()}" if re.match(r"^\d+\.|^-", line) else line for line in lines])

    def clear_conclusion(self, text: str) -> str:
        lines = text.split("\n")
        if lines:
            for phrase in ["Conclusion:", "Summary:", "Final Thoughts:"]:
                if lines[-1].startswith(phrase):
                    lines[-1] = lines[-1][len(phrase):].strip()
        return "\n".join(lines)

    def start_chat(self, user_token: str, user_input: str) -> str:
        user_info = self.fetch_user_info(user_token)
        if not user_info:
            return "Sorry, we couldn't retrieve user information. Please check your token."

        conversation_id = f"{user_info['firstName']}_{user_info['lastName']}"
        if conversation_id not in self.user_conversations:
            self.user_conversations[conversation_id] = []

        self.user_conversations[conversation_id].extend(
            [f"User: {msg}" for msg in user_info.get("lastMessages", [])[-5:]]
        )
        self.user_conversations[conversation_id].append(f"User: {user_input}")
        retrieved_answers = self.embedding.retrieve(user_input)

        response = ""
        if retrieved_answers and len(retrieved_answers[0].split()) < 50:
            response = retrieved_answers[0]
        else:
            context = " ".join(retrieved_answers[:2]) if retrieved_answers else ""
            min_chars, max_chars = self.get_dynamic_word_range(user_input, retrieved_answers)
            history = "\n".join(self.user_conversations[conversation_id][-5:])

            prompt = f"""
            You are a helpful AI assistant. Below is information about the user and recent context. Use this information only when it's directly relevant to the user's question.

            User Profile:
            - Name: {user_info['firstName']} {user_info['lastName']}
            - Location: {user_info.get('location', '')}
            - Languages: {user_info['spokenLanguages']}
            - Tags: {user_info['tags']}

            Recent Conversation History (last 5 messages):
            1. {history[-5]}
            2. {history[-4]}
            3. {history[-3]}
            4. {history[-2]}
            5. {history[-1]}

            Context (retrieved answers): {context}

            User question: {user_input}
            Answer:
            """

            response = self.generator.generate(prompt, min_chars, max_chars)
            response = self.format_response(response)
            response = self.add_bullets(response)
            response = self.clear_conclusion(response)

        if response and response[-1] not in ".!?":
            response += "."

        self.user_conversations[conversation_id].append(f"Chatbot: {response}")
        return response
    
class ConfigUpdate(BaseModel):
    hugging_face_key: Optional[str] = None
    json_path: Optional[str] = None


@app.get("/health")
def health_check():
    return JSONResponse({"status": "ok", "message": "FastAPI is running!"})
    
@app.post("/update-config")
def update_config(new_config: ConfigUpdate):
    global HUGGING_FACE_KEY
    config = load_config()

    # Update Hugging Face API key and log in if the key is provided
    if new_config.hugging_face_key:
        config["hugging_face_key"] = new_config.hugging_face_key
        HUGGING_FACE_KEY = new_config.hugging_face_key
        os.environ["HF_API_KEY"] = HUGGING_FACE_KEY
        login_result = login_hugging_face(HUGGING_FACE_KEY)
        if "error" in login_result:
            raise HTTPException(status_code=400, detail=login_result["error"])
        logger.info("Hugging Face API key updated.")

    # Update JSON path if provided
    if new_config.json_path:
        config["json_path"] = new_config.json_path
        logger.info(f"Data path updated to: {new_config.json_path}")

    # Save the updated configuration
    save_config(config)
    return {"message": "Configuration updated successfully."}

@app.get("/get-config")
def get_config():
    return load_config()

@app.on_event("startup")
async def startup_event():
    global chatbot_pipeline
    chatbot_pipeline = ChatbotPipeline(config["json_path"])  # Specify the correct path

@app.get("/chat/{user_token}/{user_input}")
async def chat(user_token: str, user_input: str):
    return {"response": chatbot_pipeline.start_chat(user_token, user_input)}

class ChatRequest(BaseModel):
    user_token: str
    user_input: str

class Chatbot:
    def __init__(self, json_path: str):
        self.pipeline = ChatbotPipeline(json_path)
    
    def get_response(self, token: str, user_input: str) -> str:
        return self.pipeline.start_chat(token, user_input)

chatbot = Chatbot(config["json_path"])

@app.post("/chat/") 
async def chat(request: ChatRequest):
    try:
        response = chatbot.get_response(request.user_token, request.user_input)
        return {"response": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))







