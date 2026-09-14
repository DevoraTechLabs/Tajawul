"use client";
import { useState } from "react";
import styles from "./TagQuestion.module.css";

export default function TagQuestion({
  question,
  options,
  selectedValues = [],
  onChange,
  required = false,
  disabled = false,
  size = "default", // 'default' or 'small'
}) {
  const [selectedTags, setSelectedTags] = useState(selectedValues);

  const handleTagClick = (value) => {
    const newSelectedTags = selectedTags.includes(value)
      ? selectedTags.filter((v) => v !== value)
      : [...selectedTags, value];

    setSelectedTags(newSelectedTags);
    onChange({ target: { name: question, value: newSelectedTags } });
  };

  return (
    <div className={`${styles.questionContainer} ${styles[size]}`}>
      <label className={styles.question}>
        {question} {required && <span className={styles.requiredMark}>*</span>}
      </label>

      <div className={styles.optionsContainer}>
        {options.map((option) => (
          <button
            key={option.value}
            className={`${styles.optionButton} ${
              selectedTags.includes(option.value) ? styles.selected : ""
            }`}
            onClick={() => !disabled && handleTagClick(option.value)}
            disabled={disabled}
          >
            <span className={styles.iconContainer}>
              {option.icon && option.icon}
              {option.label}
            </span>
          </button>
        ))}
      </div>
    </div>
  );
}
