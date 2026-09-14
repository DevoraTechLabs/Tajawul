"use client";
import styles from "./ImageUpload.module.css";
import { useState } from "react";
import Image from "next/image";
import AddPhotoAlternateIcon from "@mui/icons-material/AddPhotoAlternate";
import CloseIcon from "@mui/icons-material/Close";

export default function ImageUpload({
  label,
  id,
  required,
  onUpload,
  description,
  errorMsg,
  accept = "image/jpeg, image/jpg, image/png, image/webp", // Restrict to JPG, JPEG, PNG, WEBP
  disabled = false,
}) {
  const [filePreview, setFilePreview] = useState("");
  const [fileSizeError, setFileSizeError] = useState("");

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 2 * 1024 * 1024) {
        // 2MB limit
        setFileSizeError("Please choose an image that doesn't exceed 2 MB.");
        setFilePreview("");
        onUpload({ target: { files: [] } });
        return;
      } else {
        setFileSizeError("");
      }

      if (
        file.type === "image/jpeg" ||
        file.type === "image/jpg" ||
        file.type === "image/png" ||
        file.type === "image/webp"
      ) {
        const reader = new FileReader();
        reader.onload = (e) => {
          setFilePreview(e.target.result);
        };
        reader.readAsDataURL(file);

        // Pass the entire event object like in CoverUpload
        onUpload(event);
      } else {
        alert("Only JPG, JPEG, PNG, and WEBP files are allowed.");
        setFilePreview("");
        onUpload({ target: { files: [] } });
      }
    } else {
      setFilePreview("");
      onUpload({ target: { files: [] } });
    }
  };

  const handleRemoveImage = () => {
    setFilePreview("");
    onUpload({ target: { files: [] } });
  };

  return (
    <div className={styles.fileUploadContainer}>
      <label className={styles.fileUploadLabel} htmlFor={id}>
        {label} {required && <span className={styles.requiredMark}>*</span>}
        {description && (
          <span className={styles.fileUploadDescription}>{description}</span>
        )}
      </label>

      <div className={styles.fileUploadWrapper}>
        <input
          type="file"
          id={id}
          name={id}
          className={styles.fileInput}
          onChange={handleFileChange}
          accept={accept}
          disabled={disabled}
        />
        <label htmlFor={id} className={styles.fileUploadButton}>
          <AddPhotoAlternateIcon /> Choose File
        </label>
      </div>

      {/* Display image preview if available */}
      <div className={styles.previewContainer}>
        {filePreview && (
          <div className={styles.imagePreview}>
            <Image
              src={filePreview}
              alt="Preview"
              width={300}
              height={300}
              style={{ objectFit: "cover" }}
              priority
              className={styles.image}
            />
            <button className={styles.removeButton} onClick={handleRemoveImage}>
              <CloseIcon />
            </button>
          </div>
        )}
      </div>

      {fileSizeError && (
        <div className={styles.fileUploadError}>{fileSizeError}</div>
      )}
    </div>
  );
}
