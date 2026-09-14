"use client";

import React, { useState } from "react";
import Image from "next/image";
import styles from "./ImageList.module.css";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import API from "@/utils/api";
import { useRouter } from "next/navigation";
import ErrorMessage from "@/components/ui/ErrorMessage";

const ImageList = ({ images = [] }) => {
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();
  const destinationId =
    typeof window !== "undefined"
      ? localStorage.getItem("destinationId")
      : null;

  const handleNext = () => {
    setSelectedIndex((prevIndex) => (prevIndex + 1) % images.length);
  };

  const handlePrev = () => {
    setSelectedIndex(
      (prevIndex) => (prevIndex - 1 + images.length) % images.length
    );
  };

  const handleFileChange = async (event) => {
    const files = event.target.files;
    if (!files || files.length === 0) return;

    if (!destinationId) {
      setError("Destination ID is missing");
      return;
    }

    setLoading(true);
    setError("");

    try {
      const formData = new FormData();
      Array.from(files).forEach((file) => {
        formData.append("images", file);
      });

      const response = await API.put(
        `/Destination/${destinationId}/images`,
        formData,
        { headers: { "Content-Type": "multipart/form-data" } }
      );

      if (response.status === 200) {
        router.refresh();
      }
    } catch (err) {
      console.error("Upload failed:", err);
      setError(err.response?.data?.message || "Failed to upload images");
    } finally {
      setLoading(false);
    }
  };

  if (images.length === 0) {
    return (
      <div className={styles.emptyContainer}>
        <div className={styles.emptyIllustration}>
          <Image
            src="/no-images-upload.svg"
            alt="No images"
            width={300}
            height={300}
          />
        </div>
        <h3 className={styles.emptyTitle}>No Images Found</h3>
        <p className={styles.emptyMessage}>
          You haven't added any images yet. Upload some to bring this
          destination to life!
        </p>

        <input
          type="file"
          id="imageUploadInput"
          className={styles.hiddenInput}
          onChange={handleFileChange}
          accept="image/jpeg, image/jpg, image/png, image/webp"
          multiple
        />

        <label htmlFor="imageUploadInput" className={styles.uploadButton}>
          {loading ? (
            "Uploading..."
          ) : (
            <>
              <CloudUploadIcon className={styles.uploadIcon} />
              Upload Images
            </>
          )}
        </label>

        <p className={styles.emptyHint}>
          <strong>Supported formats:</strong> JPG, JPEG, PNG, WEBP (Max 2MB)
        </p>

        {error && (
          <div className={styles.errorMessage}>
            <ErrorMessage message={error} />
          </div>
        )}
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.mainImageWrapper}>
        {images.length > 1 && (
          <>
            <button onClick={handlePrev} className={styles.arrowLeft}>
              <ArrowBackIcon />
            </button>
            <button onClick={handleNext} className={styles.arrowRight}>
              <ArrowForwardIcon />
            </button>
          </>
        )}
        {images.length <= 5 && (
          <div className={styles.uploadButtonContainer}>
            <input
              type="file"
              id="imageUploadInput"
              className={styles.hiddenInput}
              onChange={handleFileChange}
              accept="image/jpeg, image/jpg, image/png, image/webp"
              multiple
            />
            <label
              htmlFor="imageUploadInput"
              className={styles.smolUploadButton}
            >
              {loading ? (
                "Uploading..."
              ) : (
                <>
                  <CloudUploadIcon className={styles.smolUploadIcon} />
                  Upload Images
                </>
              )}
            </label>
          </div>
        )}
        <div className={styles.mainImage}>
          <Image
            src={images[selectedIndex]}
            alt="Selected"
            layout="fill"
            objectFit="cover"
          />
        </div>
      </div>
      {images.length > 1 && (
        <div className={styles.thumbnailWrapper}>
          {images.map((image, index) => (
            <div key={index} className={styles.thumbnail}>
              <Image
                src={image}
                alt={`Thumbnail ${index}`}
                layout="fill"
                objectFit="cover"
                className={index === selectedIndex ? styles.active : ""}
                onClick={() => setSelectedIndex(index)}
              />
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ImageList;
