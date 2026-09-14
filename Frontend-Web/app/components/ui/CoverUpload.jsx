"use client";
import styles from "./ImageUpload.module.css";
import { useState } from "react";
import Image from "next/image";
import AddPhotoAlternateIcon from "@mui/icons-material/AddPhotoAlternate";
import CloseIcon from "@mui/icons-material/Close";

export default function CoverUpload({
  label,
  id,
  required,
  onUpload,
  description,
  accept = "image/jpeg, image/jpg, image/png, image/webp", // Restrict to JPG, JPEG, PNG, WEBP
  disabled = false,
}) {
  const [filePreview, setFilePreview] = useState("");
  const [fileSizeError, setFileSizeError] = useState("");

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 2 * 1024 * 1024) {
        setFileSizeError("Please choose an image that doesn't exceed 2 MB.");
        setFilePreview("");
        onUpload({ target: { files: [] } });
        return;
      } else {
        setFileSizeError(""); // Clear the error if the file is valid
      }

      if (
        file.type === "image/jpeg" ||
        file.type === "image/jpg" ||
        file.type === "image/png" ||
        file.type === "image/webp"
      ) {
        const reader = new FileReader();
        reader.onload = (e) => {
          setFilePreview(e.target.result); // Set the preview image
        };
        reader.readAsDataURL(file);

        // Pass the file object to the onUpload function
        onUpload(event); // Pass the entire event object
      } else {
        alert("Only JPG, JPEG, PNG, and WEBP files are allowed.");
        setFilePreview("");
        onUpload({ target: { files: [] } }); // Pass an empty files array
      }
    } else {
      setFilePreview("");
      onUpload({ target: { files: [] } }); // Pass an empty files array
    }
  };

  const handleRemoveImage = () => {
    setFilePreview("");
    onUpload({ target: { files: [] } }); // Pass an empty files array
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
          <AddPhotoAlternateIcon /> Upload Cover
        </label>
      </div>

      {/* Display cover preview if available */}
      <div className={styles.previewContainer}>
        {filePreview && (
          <div className={styles.coverPreview}>
            <Image
              src={filePreview}
              alt="Cover Preview"
              width={800}
              height={300}
              style={{ objectFit: "cover" }}
              priority
              className={styles.coverImage}
            />
            <button
              className={styles.coverRemoveButton}
              onClick={handleRemoveImage}
            >
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
