"use client";

import { useState, useEffect } from "react";
import styles from "./TagQuestion.module.css";
import CloseIcon from "@mui/icons-material/Close";
import Divider from "@mui/material/Divider";
import AddIcon from "@mui/icons-material/Add";

export default function EditableTagQuestion({
  question,
  options,
  selectedValues = [],
  onChange,
  onDelete,
  required = false,
  disabled = false,
  size = "default",
  addNewText = null,
  onAddNew = null, // We'll still keep this but modify its behavior
}) {
  const [selectedTags, setSelectedTags] = useState(selectedValues);
  const [deletedTags, setDeletedTags] = useState([]);
  const [isAddingNew, setIsAddingNew] = useState(false);
  const [newItemValue, setNewItemValue] = useState("");

  useEffect(() => {
    setSelectedTags(selectedValues);
  }, [selectedValues]);

  const handleTagClick = (value) => {
    let newSelectedTags;
    let newDeletedTags = [...deletedTags];

    if (selectedTags.includes(value)) {
      // Removing a tag
      newSelectedTags = selectedTags.filter((v) => v !== value);
      if (selectedValues.includes(value)) {
        // Only add to deletedTags if it was originally selected
        newDeletedTags.push(value);
      }
    } else {
      // Adding a tag
      newSelectedTags = [...selectedTags, value];
      // Remove from deletedTags if it was previously deleted
      newDeletedTags = deletedTags.filter((v) => v !== value);
    }

    setSelectedTags(newSelectedTags);
    setDeletedTags(newDeletedTags);
    onChange({ target: { name: question, value: newSelectedTags } });
    onDelete({ target: { name: question, value: newDeletedTags } });
  };

  const handleAddNewClick = () => {
    setIsAddingNew(true);
  };

  const handleNewItemSubmit = (e) => {
    e.preventDefault();
    if (newItemValue.trim()) {
      // Only notify parent about the new item, don't update selectedTags here
      if (onAddNew) {
        onAddNew({ value: newItemValue, label: newItemValue });
      }

      setNewItemValue("");
      setIsAddingNew(false);
    }
  };

  const handleNewItemCancel = () => {
    setIsAddingNew(false);
    setNewItemValue("");
  };

  // Separate selected and unselected options
  const selectedOptions = selectedTags.map((tag) => {
    const optionFromProps = options.find((opt) => opt.value === tag);
    return optionFromProps || { value: tag, label: tag };
  });

  const unselectedOptions = options.filter(
    (option) => !selectedTags.includes(option.value)
  );

  return (
    <div className={`${styles.questionContainer} ${styles[size]}`}>
      <label className={styles.question}>
        {question} {required && <span className={styles.requiredMark}>*</span>}
      </label>

      {/* Selected options section */}
      {selectedOptions.length > 0 && (
        <div className={styles.selectedSection}>
          <div className={styles.optionsContainer}>
            {selectedOptions.map((option) => {
              const wasOriginallySelected = selectedValues.includes(
                option.value
              );
              const isDeleted =
                wasOriginallySelected && !selectedTags.includes(option.value);

              return (
                <button
                  key={option.value}
                  className={`${styles.optionButton} ${styles.selected} ${
                    isDeleted ? styles.deleted : ""
                  }`}
                  onClick={() => !disabled && handleTagClick(option.value)}
                  disabled={disabled}
                >
                  <span className={styles.iconContainer}>
                    {option.icon && option.icon}
                    {option.label}
                    <span className={styles.closeIcon}>
                      <CloseIcon fontSize="small" />
                    </span>
                  </span>
                </button>
              );
            })}
          </div>
        </div>
      )}

      {/* Divider between sections */}
      {selectedOptions.length > 0 && unselectedOptions.length > 0 && (
        <Divider sx={{ width: "100%", my: 1 }} />
      )}

      {/* Unselected options section */}
      <div className={styles.unselectedSection}>
        <div className={styles.optionsContainer}>
          {unselectedOptions.map((option) => (
            <button
              key={option.value}
              className={styles.optionButton}
              onClick={() => !disabled && handleTagClick(option.value)}
              disabled={disabled}
            >
              <span className={styles.iconContainer}>
                {option.icon && option.icon}
                {option.label}
              </span>
            </button>
          ))}

          {/* Add new item input or button */}
          {addNewText && !isAddingNew && (
            <button
              className={`${styles.optionButton} ${styles.addNewButton}`}
              onClick={handleAddNewClick}
              disabled={disabled}
            >
              <span className={styles.iconContainer}>{addNewText}</span>
            </button>
          )}

          {isAddingNew && (
            <form onSubmit={handleNewItemSubmit} className={styles.addNewForm}>
              <div className={styles.inputWithButtons}>
                <input
                  type="text"
                  value={newItemValue}
                  onChange={(e) => setNewItemValue(e.target.value)}
                  className={styles.addNewInput}
                  autoFocus
                  placeholder={`New ${question.toLowerCase()}`}
                />
                <div className={styles.inputButtons}>
                  <button
                    type="submit"
                    className={styles.addButton}
                    disabled={!newItemValue.trim()}
                  >
                    <AddIcon sx={{ fontSize: "16px" }} />
                  </button>
                  <button
                    type="button"
                    onClick={handleNewItemCancel}
                    className={styles.cancelButton}
                  >
                    <CloseIcon sx={{ fontSize: "16px" }} />
                  </button>
                </div>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
