"use client";

import NavBar from "@/components/ui/NavBar";
import withAuth from "@/utils/withAuth";
import AddIcon from "@mui/icons-material/Add";
import SendIcon from "@mui/icons-material/Send";
import CustomChatIcon from "@/components/ui/CustomChatIcon";
import styles from "@/styles/Chatbot.module.css";
import useChatStore from "@/store/chatStore";
import { useState, useRef, useEffect } from "react";
import ChatLoading from "@/components/ui/ChatLoading";
import ChatHistory from "@/components/ui/ChatHistory";
import API from "@/utils/api";
import { useRouter } from "next/navigation";

function Chat() {
  const { chatId, messages, setChatId, addMessage, setMessages, clearChat } =
    useChatStore();
  const [inputValue, setInputValue] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isTyping, setIsTyping] = useState(false);
  const [displayedBotText, setDisplayedBotText] = useState("");
  const textareaRef = useRef(null);
  const messagesEndRef = useRef(null);
  const router = useRouter();

  // Auto-scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // Clear chat data when component unmounts
  useEffect(() => {
    return () => {
      clearChat();
    };
  }, [clearChat]);

  // Alternatively, you can clear the chat when the route changes
  useEffect(() => {
    const handleRouteChange = () => {
      clearChat();
    };

    router.events?.on("routeChangeStart", handleRouteChange);

    return () => {
      router.events?.off("routeChangeStart", handleRouteChange);
    };
  }, [clearChat, router.events]);

  const handleInputChange = (e) => {
    setInputValue(e.target.value);
    // Auto-resize textarea
    if (textareaRef.current) {
      textareaRef.current.style.height = "auto";
      textareaRef.current.style.height =
        textareaRef.current.scrollHeight + "px";
    }
  };

  const handleSendMessage = async () => {
    if (!inputValue.trim() || isLoading) return;

    const prompt = inputValue.trim();
    setInputValue("");
    setIsLoading(true);

    // Reset textarea height
    if (textareaRef.current) {
      textareaRef.current.style.height = "auto";
      textareaRef.current.style.height = "56px"; // Reset to default height
    }

    // Add user message
    const userMessage = { sender: "user", text: prompt };
    addMessage(userMessage);

    try {
      let response;
      if (!chatId) {
        // First message - create new chat
        response = await API.post("/Chats", { prompt });
        const { chatId: newChatId, messages: apiMessages } = response.data;
        setChatId(newChatId);
        handleTypewriterEffect(
          apiMessages[0]?.response || "No response received"
        );
      } else {
        // Continue existing chat
        response = await API.post("/Chats/prompt", { chatId, prompt });
        handleTypewriterEffect(
          response.data?.response || "No response received"
        );
      }
    } catch (error) {
      console.error("Error sending message:", error);
      handleTypewriterEffect(
        "Sorry, I couldn't process your request. Please try again later."
      );
    } finally {
      setIsLoading(false);
    }
  };

  // Helper function to handle the typewriter effect
  const handleTypewriterEffect = (fullText) => {
    setIsTyping(true);
    setDisplayedBotText(""); // Start with empty string

    let i = -1; // Start from the first character
    const interval = setInterval(() => {
      setDisplayedBotText((prev) => prev + fullText.charAt(i));
      i++;
      if (i >= fullText.length) {
        clearInterval(interval);
        setIsTyping(false);
        addMessage({
          sender: "bot",
          text: fullText,
        });
        setDisplayedBotText("");
      }
    }, 40); // you can adjust typing speed here
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  const handleNewChat = () => {
    setMessages([]);
    setChatId(null);
  };

  return (
    <div>
      <NavBar />
      <div className={styles.chatPageContainer}>
        <div className={styles.chatHistoryColumn}>
          <button className={styles.newChatButton} onClick={handleNewChat}>
            <AddIcon /> New Chat
          </button>
          <ChatHistory />
        </div>

        <div className={styles.chatInputColumn}>
          <div className={styles.chatContent}>
            {messages.length === 0 ? (
              <div className={styles.fallbackContainer}>
                <CustomChatIcon
                  size={120}
                  isFallback={true}
                  className={styles.mainIcon}
                />
                <span className={styles.chatFallback}>
                  How can I help you today?
                </span>
              </div>
            ) : (
              <div className={styles.messagesContainer}>
                {messages.map((message, index) => (
                  <div
                    key={index}
                    className={`${styles.messageWrapper} ${
                      message.sender === "user"
                        ? styles.userMessageWrapper
                        : styles.botMessageWrapper
                    }`}
                  >
                    {message.sender === "bot" && (
                      <div className={styles.botAvatar}>
                        <CustomChatIcon size={28} isActive={true} />
                      </div>
                    )}
                    <div
                      className={`${styles.message} ${
                        message.sender === "user"
                          ? styles.userMessage
                          : styles.botMessage
                      }`}
                    >
                      {message.text}
                    </div>
                  </div>
                ))}

                {isLoading && (
                  <div
                    className={`${styles.messageWrapper} ${styles.botMessageWrapper}`}
                  >
                    <div className={styles.botAvatar}>
                      <CustomChatIcon size={28} isActive={true} />
                    </div>
                    <div className={`${styles.message} ${styles.botMessage}`}>
                      <ChatLoading />
                    </div>
                  </div>
                )}
                {isTyping && (
                  <div
                    className={`${styles.messageWrapper} ${styles.botMessageWrapper}`}
                  >
                    <div className={styles.botAvatar}>
                      <CustomChatIcon size={28} isActive={true} />
                    </div>
                    <div
                      className={`${styles.message} ${styles.botMessage} ${styles.typewriterCursor}`}
                    >
                      {displayedBotText}
                    </div>
                  </div>
                )}

                <div ref={messagesEndRef} />
              </div>
            )}
          </div>

          <div className={styles.chatInput}>
            <div className={styles.inputWrapper}>
              <textarea
                ref={textareaRef}
                value={inputValue}
                onChange={handleInputChange}
                onKeyDown={handleKeyDown}
                placeholder="Ask anything..."
                className={styles.inputField}
                rows={1}
                disabled={isLoading}
              />
              <SendIcon
                className={styles.sendIcon}
                onClick={handleSendMessage}
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default withAuth(Chat);
