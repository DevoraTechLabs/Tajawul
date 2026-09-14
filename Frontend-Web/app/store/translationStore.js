// store/translationStore.js
import { create } from "zustand";

const useTranslationStore = create((set) => ({
  // Current translation state
  currentTranslation:
    typeof window !== "undefined"
      ? JSON.parse(localStorage.getItem("currentTranslation") || "null")
      : null,

  // History items
  historyItems:
    typeof window !== "undefined"
      ? JSON.parse(localStorage.getItem("translationHistory") || "[]")
      : [],

  // Grouped history by date (similar to chatDates)
  historyDates: {},

  // Set the current translation
  setCurrentTranslation: (translation) => {
    localStorage.setItem("currentTranslation", JSON.stringify(translation));
    set({ currentTranslation: translation });
  },

  // Clear current translation
  clearCurrentTranslation: () => {
    localStorage.removeItem("currentTranslation");
    set({ currentTranslation: null });
  },

  // Set history items
  setHistoryItems: (items) => {
    localStorage.setItem("translationHistory", JSON.stringify(items));
    set({ historyItems: items });
  },

  // Add to history
  addToHistory: (item) => {
    set((state) => {
      const updatedHistory = [item, ...state.historyItems];
      localStorage.setItem(
        "translationHistory",
        JSON.stringify(updatedHistory)
      );
      return { historyItems: updatedHistory };
    });
  },

  // Remove from history
  removeFromHistory: (translationId) => {
    set((state) => {
      const updatedHistory = state.historyItems.filter(
        (item) => item.translationId !== translationId
      );
      localStorage.setItem(
        "translationHistory",
        JSON.stringify(updatedHistory)
      );
      return { historyItems: updatedHistory };
    });
  },

  // Clear all history
  clearHistory: () => {
    localStorage.removeItem("translationHistory");
    set({ historyItems: [] });
  },

  // Toggle favorite status
  toggleFavorite: (translationId) => {
    set((state) => {
      const updatedHistory = state.historyItems.map((item) =>
        item.translationId === translationId
          ? { ...item, isFavorite: !item.isFavorite }
          : item
      );

      // Also update current translation if it's the one being toggled
      const updatedCurrent =
        state.currentTranslation?.translationId === translationId
          ? {
              ...state.currentTranslation,
              isFavorite: !state.currentTranslation.isFavorite,
            }
          : state.currentTranslation;

      localStorage.setItem(
        "translationHistory",
        JSON.stringify(updatedHistory)
      );
      if (updatedCurrent) {
        localStorage.setItem(
          "currentTranslation",
          JSON.stringify(updatedCurrent)
        );
      }

      return {
        historyItems: updatedHistory,
        currentTranslation: updatedCurrent,
      };
    });
  },

  // Group history by date (similar to chatDates)
  groupHistoryByDate: () => {
    set((state) => {
      // Sort history by date (newest first)
      const sortedHistory = [...state.historyItems].sort((a, b) => {
        const dateA = new Date(a.createdAt || 0);
        const dateB = new Date(b.createdAt || 0);
        return dateB - dateA;
      });

      // Group by date category
      const groupedDates = {};
      sortedHistory.forEach((item) => {
        const date = new Date(item.createdAt);
        const category = getDateCategory(date);
        if (!groupedDates[category]) {
          groupedDates[category] = [];
        }
        groupedDates[category].push(item);
      });

      return { historyDates: groupedDates };
    });
  },
}));

// Reuse the same date categorization helper from chatStore
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

export default useTranslationStore;
