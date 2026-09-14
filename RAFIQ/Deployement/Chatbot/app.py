import json
import faiss
import numpy as np
from sentence_transformers import SentenceTransformer
from transformers import AutoModelForCausalLM, AutoTokenizer, pipeline
import torch
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import logging
import requests
from typing import Dict, List, Tuple, Optional
import re
import os
import traceback
from sklearn.metrics.pairwise import cosine_similarity

app = FastAPI()

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

CONFIG_FILE = "config.json"

def load_config():
    if not os.path.exists(CONFIG_FILE):
        return {"json_path": "DATA_RQ.json"}
    with open(CONFIG_FILE, "r") as f:
        return json.load(f)

config = load_config()

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
            if isinstance(content, list):
                for entry in content:
                    if isinstance(entry, dict) and "Q" in entry and "A" in entry:
                        answer_text = entry["A"]
                        if isinstance(answer_text, list):
                            answer_text = " ".join(answer_text)
                        self.qa_pairs.append((entry["Q"], answer_text))
                    elif isinstance(entry, dict) and "question" in entry and "answer" in entry:
                        answer_text = entry["answer"]
                        if isinstance(answer_text, list):
                            answer_text = " ".join(answer_text)
                        self.qa_pairs.append((entry["question"], answer_text))
                    elif isinstance(entry, list):
                        for sub_entry in entry:
                            if isinstance(sub_entry, dict) and "question" in sub_entry and "answer" in sub_entry:
                                answer_text = sub_entry["answer"]
                                if isinstance(answer_text, list):
                                    answer_text = " ".join(answer_text)
                                self.qa_pairs.append((sub_entry["question"], answer_text))
                    else:
                        self.instructions.append(str(entry))
            elif isinstance(content, dict):
                for key, value in content.items():
                    if isinstance(value, dict) and "question" in value and "answer" in value:
                        answer_text = value["answer"]
                        if isinstance(answer_text, list):
                            answer_text = " ".join(answer_text)
                        self.qa_pairs.append((value["question"], answer_text))
                    elif isinstance(value, list):
                        self.instructions.extend(value)
                    else:
                        self.instructions.append(str(value))
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

    def retrieve(self, query: str, top_k: int = 1) -> List[Tuple[str, float]]:
        try:
            query_embedding = self.embedder.encode([query])
            distances, indices = self.index.search(np.array(query_embedding), top_k)
            retrieved = [(self.all_data[idx][1], distances[0][i]) for i, idx in enumerate(indices[0])]
            # Sanitize retrieved answers (only for retrieval, not generation)
            retrieved = [(re.sub(r'[^\x00-\x7F]+', ' ', ans).strip(), dist) for ans, dist in retrieved]
            logger.info(f"Retrieved answers for query '{query}': {[ans for ans, _ in retrieved]} with distances: {[dist for _, dist in retrieved]}")
            return retrieved
        except Exception as e:
            logger.error(f"Error retrieving answer for query '{query}': {e}")
            return []

class TextGenerator:
    def __init__(self):
        try:
            self.tokenizer = AutoTokenizer.from_pretrained("CohereForAI/aya-expanse-8b", timeout=600)
            self.model = AutoModelForCausalLM.from_pretrained(
                "CohereForAI/aya-expanse-8b", load_in_4bit=True, device_map="auto"
            )
            self.pipeline = pipeline(
                "text-generation",
                model=self.model,
                tokenizer=self.tokenizer,
                max_length=1024,  # Increased to prevent truncation
                pad_token_id=self.tokenizer.eos_token_id
            )
            logger.info("TextGenerator initialized successfully")
        except Exception as e:
            logger.error(f"Failed to initialize TextGenerator: {e}")
            raise

    def generate(self, prompt: str, min_length: int, max_length: int) -> str:
        try:
            token_count = len(self.tokenizer.encode(prompt))
            logger.debug(f"Prompt token count: {token_count}")
            if token_count > 800:  # Warn if prompt is too long
                logger.warning(f"Prompt too long ({token_count} tokens), may cause truncation")
            outputs = self.pipeline(
                prompt,
                max_length=max_length,
                min_length=min_length,
                do_sample=True,
                top_p=0.9,
                temperature=0.7,
                eos_token_id=self.tokenizer.eos_token_id,
                truncation=True
            )
            generated_text = outputs[0]['generated_text'].strip()
            output_tokens = len(self.tokenizer.encode(generated_text))
            logger.debug(f"Generated text: {generated_text[:100]}... (tokens: {output_tokens})")
            # Stop at "**Conclusion**" or "\n\nConclusion"
            if "**Conclusion**" in generated_text:
                generated_text = generated_text.split("**Conclusion**")[0].strip()
            if "\n\nConclusion" in generated_text:
                generated_text = generated_text.split("\n\nConclusion")[0].strip()
            if "\n\nPlease note" in generated_text:
                generated_text = generated_text.split("\n\nConclusion")[0].strip()      
            return generated_text
        except Exception as e:
            logger.error(f"Error generating text: {e}\nStack trace: {traceback.format_exc()}")
            return "Error generating response. Please try again."

class ChatbotPipeline:
    def __init__(self, json_path: str):
        data_loader = DataLoader(json_path)
        self.qa_pairs, self.instructions = data_loader.load()
        self.embedding = EmbeddingPipeline(self.qa_pairs, self.instructions)
        self.embedding.build_index()
        self.generator = TextGenerator()
        self.embedder = SentenceTransformer("sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2")
        self.tokenizer = self.generator.tokenizer

    def fetch_user_info(self, token: str) -> Optional[Dict]:
        url = "https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/User/info"
        headers = {"Authorization": f"Bearer {token}"}
        try:
            response = requests.get(url, headers=headers)
            response.raise_for_status()
            data = response.json()
            user_info = {
                "firstName": data.get("firstName", "User"),
                "lastName": data.get("lastName", ""),
                "tags": data.get("tags", []),
                "spokenLanguages": data.get("spokenLanguages", [])
            }
            logger.info(f"Fetched user info: {user_info}")
            return user_info
        except requests.exceptions.RequestException as e:
            logger.error(f"Error fetching user info: {e}")
            return None

    def is_question_in_knowledge_base(self, user_input: str, retrieved: List[Tuple[str, float]]) -> Optional[str]:
        if not retrieved:
            return None
        # Check if the top retrieved answer corresponds to a close question match
        top_answer, distance = retrieved[0]
        if distance < 0.1:  # Low distance indicates a very close match
            question_embedding = self.embedder.encode([user_input])
            for q, a in self.qa_pairs:
                q_embedding = self.embedder.encode([q])
                similarity = cosine_similarity(question_embedding, q_embedding)[0][0]
                if similarity > 0.95:  # High similarity threshold for exact match
                    logger.info(f"Exact match found in knowledge base for query '{user_input}': {a}")
                    return a
        return None

    def generate_response(self, user_input: str, user_info: Dict) -> str:
        retrieved = self.embedding.retrieve(user_input)
        # Check if the question is in the knowledge base
        kb_answer = self.is_question_in_knowledge_base(user_input, retrieved)
        if kb_answer:
            response_text = kb_answer
            logger.info(f"Using knowledge base answer: {response_text[:100]}... (tokens: {len(self.tokenizer.encode(response_text))})")
        else:
            context = " ".join([ans for ans, _ in retrieved])[:1000] if retrieved else ""
            prompt = f"""
            You are an intelligent, multilingual assistant. Answer the user's question using the retrieved context when relevant.
            Avoid including phrases like "Based on the context provided", "Please note", or "This is a direct translation".
            The response should be clear, concise, professional, and personalized using the user's profile.

            Profile:
            - Name: {user_info['firstName']} {user_info['lastName']}
            - Languages: {user_info['spokenLanguages']}
            - Tags: {user_info['tags']}

            Context: {context}
            User Question: {user_input}
            Answer:
            """
            num_retrieved = sum(len(ans.split()) for ans, _ in retrieved) if retrieved else 0
            num_input_words = len(user_input.split())
            avg_len = num_retrieved + num_input_words
            max_len = min(768, max(400, avg_len + 150))  # Increased to prevent truncation
            min_len = max(50, avg_len)
            response = self.generator.generate(prompt, min_length=min_len, max_length=max_len)

            if response == "Error generating response. Please try again.":
                if retrieved:
                    response_text = retrieved[0][0]
                    logger.info(f"Generation failed, using retrieved answer: {response_text[:100]}... (tokens: {len(self.tokenizer.encode(response_text))})")
                else:
                    return response
            else:
                response_text = response.split("Answer:", 1)[-1].strip() if "Answer:" in response else response.strip()

        if response_text[-1] not in ".!?":
            response_text += "."


        # Cap response at 512 tokens to avoid hallucination
        response_tokens = self.tokenizer.encode(response_text)
        if len(response_tokens) > 512:
            response_text = self.tokenizer.decode(response_tokens[:512], skip_special_tokens=True).strip()
            if not response_text.endswith(('.', '!', '?')):
                response_text += "."
            logger.warning(f"Response truncated to 512 tokens: {response_text[:100]}... (tokens: {len(self.tokenizer.encode(response_text))})")

        return response_text

class ChatRequest(BaseModel):
    user_token: str
    user_input: str

chatbot_pipeline = None

@app.on_event("startup")
async def startup_event():
    global chatbot_pipeline
    chatbot_pipeline = ChatbotPipeline(config["json_path"])

@app.get("/health")
def health_check():
    return {"status": "ok", "message": "FastAPI is running!"}

@app.post("/chat/")
async def chat(request: ChatRequest):
    try:
        user_info = chatbot_pipeline.fetch_user_info(request.user_token)
        if not user_info:
            raise HTTPException(status_code=400, detail="Could not retrieve user information. Please check your token.")
        response = chatbot_pipeline.generate_response(request.user_input, user_info)
        return {"response": response}
    except Exception as e:
        logger.error(f"Error in chat endpoint: {e}")
        raise HTTPException(status_code=500, detail=str(e))

