"use client";
import styles from "./Textarea.module.css";

export default function Textarea({
  label,
  id,
  required,
  value,
  onChange,
  description,
  errorMsg,
}) {
  return (
    <div className={styles.textareaContainer}>
      <label className={styles.textareaLabel} htmlFor={id}>
        {label} {required && <span className={styles.requiredMark}>*</span>}
        {description && (
          <span className={styles.textareaDescription}>{description}</span>
        )}
      </label>
      <textarea
        className={`${styles.textareaField} ${errorMsg ? styles.error : ""}`}
        name={id}
        id={id}
        required={required}
        value={value}
        onChange={onChange}
      />
      {errorMsg && <div className={styles.textareaError}>{errorMsg}</div>}
    </div>
  );
}
