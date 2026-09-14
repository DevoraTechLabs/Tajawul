// store/chatStore.js
import { create } from "zustand";

const useChatStore = create((set) => ({
  chatId: typeof window !== "undefined" ? localStorage.getItem("chatId") : null,
  messages:
    typeof window !== "undefined"
      ? JSON.parse(localStorage.getItem("chatMessages") || "[]")
      : [],
  chatDates: {}, // New field to store dates for chats

  // Set the entire chat state (chatId and messages)
  setChat: (chatId, messages) => {
    localStorage.setItem("chatId", chatId);
    localStorage.setItem("chatMessages", JSON.stringify(messages));
    set({ chatId, messages });
  },

  // Set just the chatId
  setChatId: (chatId) => {
    localStorage.setItem("chatId", chatId);
    set({ chatId });
  },

  // Set just the messages array
  setMessages: (messages) => {
    localStorage.setItem("chatMessages", JSON.stringify(messages));
    set({ messages });
  },

  // Add a new message to the chat
  addMessage: (message) => {
    set((state) => {
      const updatedMessages = [...state.messages, message];
      localStorage.setItem("chatMessages", JSON.stringify(updatedMessages));
      return { messages: updatedMessages };
    });
  },

  // Clear the entire chat state
  clearChat: () => {
    localStorage.removeItem("chatId");
    localStorage.removeItem("chatMessages");
    set({ chatId: null, messages: [] });
  },

  // Get the last message in the chat
  getLastMessage: () => {
    const messages = useChatStore.getState().messages;
    return messages.length > 0 ? messages[messages.length - 1] : null;
  },

  // Set chats with dates
  setChatsWithDates: (chats) => {
    // Sort chats by date (newest first)
    const sortedChats = [...chats].sort((a, b) => {
      const dateA = new Date(a.messages[0]?.createdAt || 0);
      const dateB = new Date(b.messages[0]?.createdAt || 0);
      return dateB - dateA;
    });

    // Group chats by date category
    const groupedDates = {};
    sortedChats.forEach((chat) => {
      const date = new Date(chat.messages[0]?.createdAt);
      const category = getDateCategory(date);
      if (!groupedDates[category]) {
        groupedDates[category] = [];
      }
      groupedDates[category].push(chat);
    });

    set({ chatDates: groupedDates });
  },
}));

// Helper function to categorize dates
function getDateCategory(date) {
  if (!date) return "Other";

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);

  const lastWeek = new Date(today);
  lastWeek.setDate(lastWeek.getDate() - 7);

  const inputDate = new Date(date);
  inputDate.setHours(0, 0, 0, 0);

  if (inputDate.getTime() === today.getTime()) {
    return "Today";
  } else if (inputDate.getTime() === yesterday.getTime()) {
    return "Yesterday";
  } else if (inputDate > lastWeek) {
    return "Last Week";
  } else {
    return `${inputDate.getFullYear()}-${String(
      inputDate.getMonth() + 1
    ).padStart(2, "0")}`;
  }
}

export default useChatStore;
