"use client";
import styles from "./Input.module.css";

export default function Input({
  label,
  id,
  required,
  type,
  value,
  onChange,
  description,
  errorMsg,
}) {
  return (
    <div className={styles.inputContainer}>
      <label className={styles.inputLabel} htmlFor={id}>
        {label} {required && <span className={styles.requiredMark}>*</span>}
        {description && (
          <span className={styles.inputDescription}>{description}</span>
        )}
      </label>
      <input
        className={`${styles.inputField} ${errorMsg ? styles.error : ""}`}
        type={type}
        name={id}
        id={id}
        required={required}
        value={value}
        onChange={onChange}
      />
      {errorMsg && <div className={styles.inputError}>{errorMsg}</div>}
    </div>
  );
}
