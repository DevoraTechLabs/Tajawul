import { useState, useEffect } from "react";
import useChatStore from "@/store/chatStore";
import API from "@/utils/api";
import styles from "@/styles/Chatbot.module.css";
import ChatOutlinedIcon from "@mui/icons-material/ChatOutlined";
import DeleteOutlinedIcon from "@mui/icons-material/DeleteOutlined";
import { WhiteLoading } from "@/components/ui/Loading";

const ChatCard = ({ chat, isActive, onClick, onDelete }) => {
  const prompt = chat.messages[0]?.prompt || "New Chat";
  const truncatedPrompt =
    prompt.length > 30 ? `${prompt.substring(0, 30)}...` : prompt;

  return (
    <div
      className={`${styles.chatCard} ${isActive ? styles.activeChatCard : ""}`}
      onClick={onClick}
    >
      <div className={styles.chatCardContent}>
        <div className={styles.chatCardTitle}>
          <ChatOutlinedIcon
            className={`${styles.chatCardIcon} ${
              isActive ? styles.activeIcon : ""
            }`}
          />
          <p
            className={`${styles.chatCardPrompt} ${
              isActive ? styles.activeText : ""
            }`}
          >
            {truncatedPrompt}
          </p>
        </div>
        <DeleteOutlinedIcon
          className={`${styles.deleteIcon} ${
            isActive ? styles.activeIcon : ""
          }`}
          onClick={(e) => {
            e.stopPropagation();
            onDelete(chat.chatId);
          }}
        />
      </div>
    </div>
  );
};

const ChatHistory = () => {
  const { chatId, chatDates, setChatsWithDates } = useChatStore();
  const [loading, setLoading] = useState(true);
  const { setChatId, setMessages } = useChatStore();
  const [deleteId, setDeleteId] = useState(null);
  const [showConfirm, setShowConfirm] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await API.get("/Chats");
        setChatsWithDates(response.data);
      } catch (error) {
        console.error("Error fetching chats:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchChats();
  }, [setChatsWithDates]);

  const handleChatClick = (chat) => {
    setChatId(chat.chatId);
    const formattedMessages = chat.messages.map((msg) => ({
      sender: "user",
      text: msg.prompt,
    }));
    if (chat.messages[0]?.response) {
      formattedMessages.push({
        sender: "bot",
        text: chat.messages[0].response,
      });
    }
    setMessages(formattedMessages);
  };

  const handleDelete = (chatId) => {
    setDeleteId(chatId);
    setShowConfirm(true);
  };

  const confirmDelete = async () => {
    setIsDeleting(true);
    try {
      await API.delete("/Chats", { data: { chatId: deleteId } });
      // Refetch chats to update the list
      const response = await API.get("/Chats");
      setChatsWithDates(response.data);
      if (chatId === deleteId) {
        setChatId(null);
        setMessages([]);
      }
    } catch (error) {
      console.error("Error deleting chat:", error);
    } finally {
      setIsDeleting(false);
      setShowConfirm(false);
      setDeleteId(null);
    }
  };

  if (loading) {
    return <div className={styles.loadingWrapper}>Loading chats...</div>;
  }

  return (
    <>
      <div className={styles.chatHistoryList}>
        {Object.entries(chatDates).map(([category, chats]) => (
          <div key={category} className={styles.chatDateGroup}>
            <div className={styles.chatDateCategory}>{category}</div>
            {chats.map((chat) => (
              <ChatCard
                key={chat.chatId}
                chat={chat}
                isActive={chatId === chat.chatId}
                onClick={() => handleChatClick(chat)}
                onDelete={handleDelete}
              />
            ))}
          </div>
        ))}
      </div>
      {showConfirm && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            width: "100vw",
            height: "100vh",
            background: "rgba(0,0,0,0.3)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            zIndex: 9999,
          }}
        >
          <div
            style={{
              background: "var(--Neutrals-Background)",
              border: "1px solid var(--Neutrals-Light-Outline)",
              padding: "32px",
              borderRadius: "12px",
              boxShadow: "0 2px 16px rgba(0,0,0,0.15)",
              minWidth: "320px",
              textAlign: "center",
            }}
          >
            <p
              style={{
                marginBottom: "20px",
                fontSize: "18px",
                fontWeight: "600",
                fontFamily: "var(--font-body)",
              }}
            >
              Are you sure you want to delete this chat?
            </p>
            <div style={{ display: "flex", justifyContent: "center" }}>
              <button
                style={{
                  padding: "8px 20px",
                  marginRight: "16px",
                  borderRadius: "8px",
                  fontSize: "16px",
                  fontWeight: "500",
                  border: "1px solid var(--Neutrals-Light-Outline)",
                  background: "var(--Neutrals-Background)",
                  color: "var(--Neutrals-Medium-Outline)",
                  cursor: "pointer",
                  fontFamily: "var(--font-body)",
                  transition: "all 0.2s ease-in-out",
                }}
                onClick={() => setShowConfirm(false)}
                onMouseOver={(e) => {
                  e.currentTarget.style.background = "var(--Beige-Hover)";
                  e.currentTarget.style.color = "var(--Neutrals-Black-Text)";
                }}
                onMouseOut={(e) => {
                  e.currentTarget.style.background =
                    "var(--Neutrals-Background)";
                  e.currentTarget.style.color =
                    "var(--Neutrals-Medium-Outline)";
                }}
              >
                Cancel
              </button>
              <button
                style={{
                  padding: "8px 20px",
                  borderRadius: "8px",
                  fontSize: "16px",
                  border: "none",
                  fontWeight: "500",
                  background: isDeleting
                    ? "var(--Error-Text)"
                    : "var(--Neutrals-Background)",
                  color: isDeleting
                    ? "var(--Neutrals-Background)"
                    : "var(--Error-Text)",
                  border: "1px solid rgb(202, 44, 44)",
                  cursor: isDeleting ? "not-allowed" : "pointer",
                  fontFamily: "var(--font-body)",
                  transition: "all 0.2s ease-in-out",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  gap: "8px",
                }}
                onClick={!isDeleting ? confirmDelete : undefined}
                onMouseOver={(e) => {
                  if (!isDeleting) {
                    e.currentTarget.style.background = "var(--Error-Text)";
                    e.currentTarget.style.color = "var(--Neutrals-Background)";
                  }
                }}
                onMouseOut={(e) => {
                  if (!isDeleting) {
                    e.currentTarget.style.background =
                      "var(--Neutrals-Background)";
                    e.currentTarget.style.color = "var(--Error-Text)";
                  }
                }}
                disabled={isDeleting}
              >
                {isDeleting ? (
                  <>
                    <WhiteLoading size={16} />
                    Deleting...
                  </>
                ) : (
                  "Delete"
                )}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default ChatHistory;
