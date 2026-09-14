"use client";
import { useState, useEffect, useRef } from "react";
import styles from "./Dropdown.module.css";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import CloseIcon from "@mui/icons-material/Close";

export default function MultiDropdown({
  label,
  id,
  required,
  value = [],
  onChange,
  options,
  description,
  errorMsg,
  placeholder = "Select options",
  disabled = false,
}) {
  const [isOpen, setIsOpen] = useState(false);
  const [searchText, setSearchText] = useState("");
  const dropdownRef = useRef(null);

  // Sort options alphabetically by label
  const sortedOptions = [...options].sort((a, b) =>
    a.label.localeCompare(b.label)
  );

  const filteredOptions = sortedOptions.filter((opt) =>
    opt.label.toLowerCase().includes(searchText.toLowerCase())
  );

  const toggleDropdown = () => {
    if (!disabled) setIsOpen((prev) => !prev);
  };

  const handleSelect = (selectedValue) => {
    const newValue = value.includes(selectedValue)
      ? value.filter((v) => v !== selectedValue)
      : [...value, selectedValue];
    onChange({ target: { name: id, value: newValue } });
  };

  const handleRemove = (selectedValue) => {
    const newValue = value.filter((v) => v !== selectedValue);
    onChange({ target: { name: id, value: newValue } });
  };

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    } else {
      document.removeEventListener("mousedown", handleClickOutside);
    }

    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [isOpen]);

  return (
    <div className={styles.dropdownContainer} ref={dropdownRef}>
      <label className={styles.dropdownLabel} htmlFor={id}>
        {label} {required && <span className={styles.requiredMark}>*</span>}
        {description && (
          <span className={styles.dropdownDescription}>{description}</span>
        )}
      </label>

      <div
        className={`${styles.dropdownWrapper} ${isOpen ? styles.active : ""} ${
          disabled ? styles.disabled : ""
        }`}
        onClick={toggleDropdown}
      >
        <input
          type="text"
          className={styles.searchInput}
          value={searchText}
          onChange={(e) => {
            if (!disabled) {
              setSearchText(e.target.value);
              setIsOpen(true);
            }
          }}
          placeholder={placeholder}
          autoComplete="off"
          disabled={disabled}
        />

        <KeyboardArrowDownIcon
          className={`${styles.dropdownIcon} ${isOpen ? styles.rotated : ""}`}
        />
      </div>

      {isOpen && (
        <div className={styles.dropdownMenu}>
          {filteredOptions.length > 0 ? (
            filteredOptions.map((option, index) => (
              <div
                key={index}
                className={`${styles.dropdownItem} ${
                  value.includes(option.value) ? styles.selected : ""
                }`}
                onClick={() => handleSelect(option.value)}
              >
                {option.label}
              </div>
            ))
          ) : (
            <div className={styles.noResults}>No results found</div>
          )}
        </div>
      )}

      <div className={styles.selectedOptions}>
        {value.map((selectedValue) => {
          const selectedOption = options.find(
            (opt) => opt.value === selectedValue
          );
          return (
            <div key={selectedValue} className={styles.selectedOption}>
              {selectedOption?.label}
              <CloseIcon
                className={styles.removeIcon}
                onClick={() => handleRemove(selectedValue)}
              />
            </div>
          );
        })}
      </div>

      {errorMsg && <div className={styles.dropdownError}>{errorMsg}</div>}
    </div>
  );
}
