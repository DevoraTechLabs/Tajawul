"use client";

import { useState, useEffect } from "react";
import styles from "@/styles/Translate.module.css";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import FavoriteIcon from "@mui/icons-material/Favorite";
import HistoryIcon from "@mui/icons-material/History";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import ErrorMessage from "@/components/ui/ErrorMessage";
import DeleteIcon from "@mui/icons-material/Delete";
import TranslationDropdown from "@/components/ui/MUIdropdown/TranslationDropdown";
import API from "@/utils/api";
import withAuth from "@/utils/withAuth";
import useTranslationStore from "@/store/translationStore";

function Translate() {
  const {
    currentTranslation,
    historyItems,
    setCurrentTranslation,
    clearCurrentTranslation,
    setHistoryItems,
    addToHistory,
    removeFromHistory,
    clearHistory,
    toggleFavorite,
    groupHistoryByDate,
  } = useTranslationStore();

  const [fromLanguage, setFromLanguage] = useState("Choose Language");
  const [toLanguage, setToLanguage] = useState("Choose Language");
  const [fromLanguageCode, setFromLanguageCode] = useState("");
  const [toLanguageCode, setToLanguageCode] = useState("");
  const [sourceText, setSourceText] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [showCopied, setShowCopied] = useState(false);
  const [showHistory, setShowHistory] = useState(false);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [showFavorites, setShowFavorites] = useState(false);
  const [favoritesLoading, setFavoritesLoading] = useState(false);
  const [favoritesItems, setFavoritesItems] = useState([]);

  // Initialize from current translation if exists
  useEffect(() => {
    if (currentTranslation) {
      setSourceText(currentTranslation.sourceText);
      setFromLanguageCode(currentTranslation.inputLanguage);
      setToLanguageCode(currentTranslation.outputLanguage);
    }
  }, [currentTranslation]);

  const handleFromLanguageSelect = (languageName, languageCode) => {
    setFromLanguage(languageName);
    setFromLanguageCode(languageCode);
    setError("");
  };

  const handleToLanguageSelect = (languageName, languageCode) => {
    setToLanguage(languageName);
    setToLanguageCode(languageCode);
    setError("");
  };

  const handleTranslate = async () => {
    if (!fromLanguageCode || !toLanguageCode) {
      setError("Please select both input and output languages");
      return;
    }

    if (!sourceText.trim()) {
      setError("Please enter text to translate");
      return;
    }

    setError("");
    setIsLoading(true);
    clearCurrentTranslation(); // Clear current translation before new request

    try {
      const response = await API.post("/Translation", {
        sourceText: sourceText,
        inputLanguage: fromLanguageCode,
        outputLanguage: toLanguageCode,
      });

      const translation = response.data;
      setCurrentTranslation(translation);
      addToHistory(translation);
    } catch (error) {
      console.error("Translation error:", error);
      setError("Translation failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  // Cleanup effect when leaving the page
  useEffect(() => {
    return () => {
      clearCurrentTranslation(); // Clear when component unmounts
    };
  }, [clearCurrentTranslation]);

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleTranslate();
    }
  };

  const handleCopy = () => {
    if (!currentTranslation?.translatedText) return;

    navigator.clipboard
      .writeText(currentTranslation.translatedText)
      .then(() => {
        setShowCopied(true);
        setTimeout(() => setShowCopied(false), 2000);
      })
      .catch((err) => {
        console.error("Failed to copy text: ", err);
      });
  };

  const handleToggleFavorite = async () => {
    if (!currentTranslation?.translationId) return;

    try {
      await API.patch("/Translation/toggle-favorite", {
        translationItemId: currentTranslation.translationId,
      });
      toggleFavorite(currentTranslation.translationId);
    } catch (error) {
      console.error("Error toggling favorite:", error);
      setError("Failed to update favorite status");
    }
  };

  // Fetch history when showHistory changes
  useEffect(() => {
    if (showHistory) {
      fetchHistory();
    }
  }, [showHistory]);

  const fetchHistory = async () => {
    setHistoryLoading(true);
    try {
      const response = await API.get(
        "/Translation?PageNumber=1&PageSize=20&SortDescending=true"
      );
      setHistoryItems(response.data);
      groupHistoryByDate();
    } catch (error) {
      console.error("Error fetching history:", error);
      setError("Failed to load history");
    } finally {
      setHistoryLoading(false);
    }
  };

  const toggleHistory = () => {
    setShowHistory(!showHistory);
  };

  const handleClearHistory = async () => {
    try {
      await API.delete("/Translation");
      clearHistory();
    } catch (error) {
      console.error("Error clearing history:", error);
      setError("Failed to clear history");
    }
  };

  const handleDeleteHistoryItem = async (translationId) => {
    try {
      await API.delete(`/Translation/translation-item`, {
        data: { translationItemId: translationId },
      });
      removeFromHistory(translationId);
    } catch (error) {
      console.error("Error deleting history item:", error);
      setError("Failed to delete history item");
    }
  };

  const fetchFavorites = async () => {
    setFavoritesLoading(true);
    try {
      const response = await API.get(
        "/Translation/favorites?PageNumber=1&PageSize=20&SortDescending=true"
      );
      setFavoritesItems(response.data);
    } catch (error) {
      console.error("Error fetching favorites:", error);
      setError("Failed to load favorites");
    } finally {
      setFavoritesLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.buttonsContainer}>
        <button
          className={`${styles.actionsButton} ${
            showFavorites ? styles.activeButton : ""
          }`}
          onClick={() => {
            setShowFavorites(!showFavorites);
            if (!showFavorites) fetchFavorites();
            setShowHistory(false);
          }}
        >
          <FavoriteIcon />
          Favorites
        </button>
        <button
          className={`${styles.actionsButton} ${
            showHistory ? styles.activeButton : ""
          }`}
          onClick={toggleHistory}
        >
          <HistoryIcon />
          History
        </button>
      </div>
      <div className={styles.translateInputsContainer}>
        <div className={styles.translateCard}>
          <div className={styles.chooseLanguageContainer}>
            <span className={styles.chooseLanguageText}> From:</span>
            <TranslationDropdown
              selectedLanguage={fromLanguage}
              onLanguageSelect={handleFromLanguageSelect}
            />
          </div>
          <div className={styles.translateInputContainer}>
            <textarea
              className={styles.translateInput}
              placeholder="Type here..."
              value={sourceText}
              onChange={(e) => setSourceText(e.target.value)}
              onKeyDown={handleKeyDown}
            ></textarea>
          </div>

          <div className={styles.translateButtonContainer}>
            <div className={styles.errorContainer}>
              {error && <ErrorMessage message={error} />}
            </div>
            <button
              className={styles.translateButton}
              onClick={handleTranslate}
              disabled={isLoading}
            >
              {isLoading ? "Translating..." : "Translate"}
            </button>
          </div>
        </div>
        <div className={styles.translateCard}>
          <div className={styles.chooseLanguageContainer}>
            <span className={styles.chooseLanguageText}> To:</span>
            <TranslationDropdown
              selectedLanguage={toLanguage}
              onLanguageSelect={handleToLanguageSelect}
            />
          </div>
          <div className={styles.translateOutputContainer}>
            <textarea
              readOnly
              className={styles.translateInput}
              placeholder="Translated text appears here."
              value={currentTranslation?.translatedText || ""}
            ></textarea>
            <div className={styles.textareaButtons}>
              <button
                className={styles.copyButton}
                onClick={handleCopy}
                disabled={!currentTranslation?.translatedText}
                aria-label="Copy translation"
              >
                {showCopied && (
                  <span className={styles.copiedText}>Copied!</span>
                )}
                <ContentCopyIcon sx={{ fontSize: 20 }} />
              </button>
              <button
                className={`${styles.favoriteTextareaButton} ${
                  currentTranslation?.isFavorite
                    ? styles.favoriteTextareaActive
                    : ""
                }`}
                onClick={handleToggleFavorite}
                disabled={!currentTranslation?.translatedText}
                aria-label={
                  currentTranslation?.isFavorite
                    ? "Remove from favorites"
                    : "Add to favorites"
                }
              >
                {currentTranslation?.isFavorite ? (
                  <FavoriteIcon
                    sx={{
                      fontSize: 20,
                      color: "var(--Green-Hover)",
                      transition: "color 0.3s ease",
                    }}
                  />
                ) : (
                  <FavoriteBorderIcon
                    sx={{
                      fontSize: 20,
                      transition: "color 0.3s ease",
                    }}
                  />
                )}
              </button>
            </div>
          </div>
          <div className={styles.translateButtonContainer}></div>
        </div>
      </div>
      {showFavorites && (
        <div className={styles.favoritesContainer}>
          <div className={styles.favoritesHeader}>
            <h1>Favorites</h1>
            <div className={styles.favoritesControls}>
              <div className={styles.favoritesPagination}>
                <span>
                  1-{favoritesItems.length} of {favoritesItems.length} Phrases
                </span>
              </div>
            </div>
          </div>

          {favoritesLoading ? (
            <div className={styles.loading}>Loading favorites...</div>
          ) : (
            <div className={styles.favoritesGrid}>
              {favoritesItems.map((item) => (
                <div key={item.translationId} className={styles.favoritesItem}>
                  <div className={styles.itemContent}>
                    <span className={styles.languageTag}>
                      {item.inputLanguage} → {item.outputLanguage}
                    </span>
                    <div className={styles.textPair}>
                      <p className={styles.sourceText}>{item.sourceText}</p>
                      <p className={styles.arrowText}> → </p>
                      <p className={styles.translatedText}>
                        {item.translatedText}
                      </p>
                    </div>
                  </div>
                  <div className={styles.itemActions}>
                    <button
                      className={styles.favoriteButton}
                      onClick={async () => {
                        try {
                          await API.patch("/Translation/toggle-favorite", {
                            translationItemId: item.translationId,
                          });
                          toggleFavorite(item.translationId);
                          fetchFavorites(); // Refresh favorites list
                        } catch (error) {
                          console.error("Error toggling favorite:", error);
                          setError("Failed to update favorite status");
                        }
                      }}
                      aria-label="Remove from favorites"
                    >
                      <FavoriteIcon
                        sx={{
                          fontSize: "20px",
                          color: "var(--Green-Hover)",
                        }}
                      />
                    </button>
                    <button
                      className={styles.deleteButton}
                      onClick={() =>
                        handleDeleteHistoryItem(item.translationId)
                      }
                      aria-label="Delete translation"
                    >
                      <DeleteIcon fontSize="small" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
      {showHistory && (
        <div className={styles.historyContainer}>
          <div className={styles.historyHeader}>
            <h1>History</h1>
            <div className={styles.historyControls}>
              <div className={styles.historyPagination}>
                <span>
                  1-{historyItems.length} of {historyItems.length} Phrases
                </span>
              </div>
              <button
                className={styles.clearButton}
                onClick={handleClearHistory}
              >
                Clear all History
              </button>
            </div>
          </div>

          {historyLoading ? (
            <div className={styles.loading}>Loading history...</div>
          ) : (
            <div className={styles.historyGrid}>
              {historyItems.map((item) => (
                <div key={item.translationId} className={styles.historyItem}>
                  <div className={styles.itemContent}>
                    <span className={styles.languageTag}>
                      {item.inputLanguage} → {item.outputLanguage}
                    </span>
                    <div className={styles.textPair}>
                      <p className={styles.sourceText}>{item.sourceText}</p>
                      <p className={styles.arrowText}> → </p>
                      <p className={styles.translatedText}>
                        {item.translatedText}
                      </p>
                    </div>
                  </div>
                  <div className={styles.itemActions}>
                    <button
                      className={styles.favoriteButton}
                      onClick={async () => {
                        try {
                          await API.patch("/Translation/toggle-favorite", {
                            translationItemId: item.translationId,
                          });
                          toggleFavorite(item.translationId);
                        } catch (error) {
                          console.error("Error toggling favorite:", error);
                          setError("Failed to update favorite status");
                        }
                      }}
                      aria-label={
                        item.isFavorite
                          ? "Remove from favorites"
                          : "Add to favorites"
                      }
                    >
                      {item.isFavorite ? (
                        <FavoriteIcon
                          sx={{
                            fontSize: "20px",
                            color: "var(--Green-Hover)",
                          }}
                        />
                      ) : (
                        <FavoriteBorderIcon fontSize="small" />
                      )}
                    </button>
                    <button
                      className={styles.deleteButton}
                      onClick={() =>
                        handleDeleteHistoryItem(item.translationId)
                      }
                      aria-label="Delete translation"
                    >
                      <DeleteIcon fontSize="small" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default withAuth(Translate);
