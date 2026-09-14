"use client";
import styles from "./TagQuestion.module.css";

export default function InterestsTagQuestion({
  question,
  options,
  selectedValues = [],
  onChange,
  required = false,
  disabled = false,
  size = "default", // 'default' or 'small'
  description,
  maxSelections, // Now fully customizable with no default
}) {
  const handleTagClick = (value) => {
    if (disabled) return;

    const newSelectedTags = selectedValues.includes(value)
      ? selectedValues.filter((v) => v !== value)
      : [...selectedValues, value];
    onChange(newSelectedTags);
  };

  // Check if max selections reached (only if maxSelections is defined)
  const isMaxSelectionsReached =
    maxSelections !== undefined && selectedValues.length >= maxSelections;
  // Check if an option is not selected (used for disabling)
  const isOptionDisabled = (value) =>
    disabled || (isMaxSelectionsReached && !selectedValues.includes(value));

  return (
    <div className={`${styles.questionContainer} ${styles[size]}`}>
      <label className={styles.question}>
        {question} {required && <span className={styles.requiredMark}>*</span>}
        {description && (
          <div className={styles.descriptionContainer}>
            <span className={styles.description}>{description}</span>
            {isMaxSelectionsReached && (
              <div className={styles.selectionLimitMessage}>
                Maximum {maxSelections} selections reached
              </div>
            )}
          </div>
        )}
      </label>

      <div className={styles.optionsContainer}>
        {options.map((option) => (
          <button
            key={option.value}
            type="button"
            className={`${styles.optionButton} ${
              selectedValues.includes(option.value) ? styles.selected : ""
            } ${isOptionDisabled(option.value) ? styles.disabled : ""}`}
            onClick={() => handleTagClick(option.value)}
            disabled={isOptionDisabled(option.value)}
            aria-disabled={isOptionDisabled(option.value)}
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
