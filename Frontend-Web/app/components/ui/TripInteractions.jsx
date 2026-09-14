"use client";

import { useState, useEffect } from "react";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import FavoriteIcon from "@mui/icons-material/Favorite";
import BookmarkBorderIcon from "@mui/icons-material/BookmarkBorder";
import BookmarkIcon from "@mui/icons-material/Bookmark";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import CameraAltIcon from "@mui/icons-material/CameraAlt";
import API from "@/utils/api";
import useAuthStore from "@/store/authStore";
import styles from "@/trip.module.css";
import { SmolGreenLoading } from "./Loading";
import Image from "next/image";
import useTripInteractionsStore from "@/store/TripInteractionsStore";
import CloseIcon from "@mui/icons-material/Close";
import SuccessMessage from "@/components/ui/SuccessMessage";
import ErrorMessage from "@/components/ui/ErrorMessage";

const TripInteractions = ({ tripId, onCoverUpdate }) => {

  const [interactions, setInteractions] = useState({

  favorite: false,
  wish: false,
  clone: false,
  });

  const [loading, setLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const { accessToken, roles } = useAuthStore();
  const [showCoverUpload, setShowCoverUpload] = useState(false);
  const [filePreview, setFilePreview] = useState("");
  const [uploading, setUploading] = useState(false);
  const [showProtectedPopup, setShowProtectedPopup] = useState(false);

const { setFavoritesCount, setWishesCount, setClonesCount } = useTripInteractionsStore();

  useEffect(() => {
    const fetchInteractionStatus = async () => {
      try {
        const response = await API.get(`/user/${tripId}/tripStatus`);
        if (response.data) {
          setInteractions({
            favorite: !!response.data.favorite,
            wish: !!response.data.wish,
            clone: !!response.data.clone,

          });
        }
      } catch (error) {
        console.error("Failed to fetch interaction status:", error);
      } finally {
        setLoading(false);
      }
    };

    if (accessToken) {
      fetchInteractionStatus();
    }
  }, [tripId, accessToken]);

  const handleInteraction = async (type) => {
  if (!roles.includes("User")) {
    setShowProtectedPopup(true);
    return;
  }

  const currentValue = interactions[type];
  setInteractions((prev) => ({ ...prev, [type]: !currentValue }));

  try {
    const res = currentValue      
      ? await API.delete(`/user/${tripId}/${type}`)
      : await API.post(`/user/${tripId}/${type}`);

    if (res?.data) {
      if (res.data.favoritesCount !== undefined)
        setFavoritesCount(res.data.favoritesCount);
      if (res.data.wishesCount !== undefined)
        setWishesCount(res.data.wishesCount);
      if (res.data.clonesCount !== undefined)
        setClonesCount(res.data.clonesCount);
    }
  } catch (error) {
    console.error(`Failed to update ${type} status:`, error);
    setInteractions((prev) => ({ ...prev, [type]: currentValue })); // revert on fail
  }
};
        

  useEffect(() => {
    if (tripId) {
      useTripInteractionsStore.getState().initialize(tripId);
    }
  }, [tripId]);
  const handleFileChange = (event) => {
    const file = event.target.files?.[0];
    setErrorMessage(""); // Clear any previous errors

    if (!file) {
      setFilePreview("");
      return;
    }

    // Validate file type
    const validTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
    if (!validTypes.includes(file.type)) {
      showError("Only JPG, JPEG, PNG, and WEBP files are allowed");
      return;
    }

    // Validate file size
    if (file.size > 2 * 1024 * 1024) {
      showError("File size must be less than 2MB");
      return;
    }

    // Create preview
    const reader = new FileReader();
    reader.onload = (e) => setFilePreview(e.target.result);
    reader.readAsDataURL(file);
  };

  const handleRemoveImage = () => {
    // Clear both preview and input
    setFilePreview("");
    const fileInput = document.getElementById('cover-upload');
    if (fileInput) {
      fileInput.value = "";
    }
    setErrorMessage("");
  };

  const handleUpload = async () => {
  if (!filePreview) {
    showError("Please select an image first");
    return;
  }

  try {
    setUploading(true);
    const fileInput = document.querySelector(`#cover-upload`);
    
    if (!fileInput?.files?.[0]) {
      showError("No file selected");
      return;
    }

    const formData = new FormData();
    formData.append('ProfileImage', fileInput.files[0]);

    const response = await API.put(
      `/Trip/${tripId}/coverImage`,
      formData,
      { 
        headers: { 'Content-Type': 'multipart/form-data' },
        timeout: 30000
      }
    );

    if (response?.data) {
      let newCover = response.data.coverImage || response.data.imageUrl || response.data.url;
      
      if (!newCover) {
        const tripRes = await API.get(`/Trip?TripId=${tripId}`);
        newCover = tripRes.data.trips?.find(t => t.tripId === tripId)?.coverImage;
      }

      if (newCover) {
        // Add timestamp to prevent caching
        const timestampedUrl = `${newCover}?t=${Date.now()}`;
        onCoverUpdate?.(timestampedUrl);
        setShowCoverUpload(false);
        setFilePreview("");
        setSuccessMessage("Cover image updated successfully!");
        return;
      }
    }
    
    showError("Upload completed but no image URL was returned");
  } catch (error) {
    console.error("Upload failed:", error);
    showError(error.response?.data?.message || error.message || "Upload failed. Please try again.");
  } finally {
    setUploading(false);
  }
};
  if (loading) {
    return (
      <>
        <button className={styles.saveButton}>
          <SmolGreenLoading />
        </button>
        <button className={styles.saveButton}>
          <SmolGreenLoading />
        </button>
        <button className={styles.saveButton}>
          <SmolGreenLoading />
        </button>
      </>
    );
  }

 if (!accessToken) {
   return (
     <button
       className={styles.saveButton}
       onClick={() => (window.location.href = "/login")}
     >
       Sign in to interact
     </button>
   );
 }

  return (
    <>
      {/* Success and error messages */}
      {successMessage && !showCoverUpload && (
        <div className={styles.notificationContainer}>
          <SuccessMessage 
            message={successMessage} 
            onClose={() => setSuccessMessage("")}
          />
        </div>
      )}
      {errorMessage && !showCoverUpload && (
        <div className={styles.notificationContainer}>
          <ErrorMessage 
            message={errorMessage} 
            onClose={() => setErrorMessage("")}
          />
        </div>
      )}

      <div className={styles.buttomRightContainer}>
        <button 
          className={`${styles.saveButton} ${styles.cameraButton}`}
          onClick={() => setShowCoverUpload(true)}
        >
          <CameraAltIcon />
        </button>

     <button
       className={`${styles.saveButton} ${
         interactions.favorite ? styles.activeButton : ""
       }`}
       onClick={() => handleInteraction("favorite")}
     >
       {interactions.favorite ? <FavoriteIcon /> : <FavoriteBorderIcon />}
       {interactions.favorite ? "Favorited" : "Favorite"}
     </button>

    <button
      className={`${styles.saveButton} ${
        interactions.wish ? styles.activeButton : ""
      }`}
      onClick={() => handleInteraction("wish")}
    >
      {interactions.wish ? <FavoriteIcon /> : <FavoriteBorderIcon />}
      {interactions.wish ? "Wished" : "Wish"}
    </button>

    <button
      className={`${styles.saveButton} ${
        interactions.clone ? styles.activeButton : ""
      }`}
      onClick={() => handleInteraction("clone")}
    >
      {interactions.clone ? <ContentCopyIcon /> : <ContentCopyIcon />}
      {interactions.clone ? "Cloned" : "Clone"}
    </button>
      </div>

    {showCoverUpload && (
      <div className={styles.popupOverlay}>
        <div className={styles.popupContainer}>
          <h2>Update Cover Image</h2>
          <p className={styles.popupDescription}>
            Accepted formats: JPG, JPEG, PNG, WEBP<br />
            Maximum file size: 2MB
          </p>

          {errorMessage && (
            <div className={styles.fileUploadError}>
              {errorMessage}
            </div>
          )}

          <div className={styles.uploadArea}>
            <input
              type="file"
              id="cover-upload"
              accept="image/jpeg, image/jpg, image/png, image/webp"
              onChange={handleFileChange}
              className={styles.fileInput}
            />
            <label htmlFor="cover-upload" className={styles.uploadCoverButton}>
              <CameraAltIcon />
              {filePreview ? "Change Image" : "Upload Cover"}
            </label>
          </div>

          {filePreview && (
            <div className={styles.previewContainer}>
              <img 
                src={filePreview} 
                alt="Preview" 
                className={styles.imagePreview}
              />
              <button
                className={styles.coverRemoveButton}
                onClick={handleRemoveImage}
              >
                <CloseIcon />
              </button>
            </div>
          )}

          <div className={styles.popupButtons}>
            <button 
              className={styles.popupCancelButton}
              onClick={() => {
                setShowCoverUpload(false);
                setFilePreview("");
                setErrorMessage("");
              }}
            >
              Cancel
            </button>
            <button 
              className={styles.popupConfirmButton}
              onClick={handleUpload}
              disabled={!filePreview || uploading}
            >
              {uploading ? "Uploading..." : "Upload"}
            </button>
          </div>
        </div>
      </div>
    )}
      {showProtectedPopup && (
        <div className={styles.popupOverlay}>
          <div className={styles.popupContent}>
            <button
              className={styles.closeButton}
              onClick={() => setShowProtectedPopup(false)}
            >
              ✕
            </button>
            <div className={styles.completedStep}>
              <Image
                src="/protected-feature.svg"
                alt="Protected feature"
                width={450}
                height={350}
                className={styles.completedImage}
              />
              <button
                className={styles.ctaButton}
                onClick={() =>
                  (window.location.href = "/complete-your-profile")
                }
              >
                Complete Your Profile to Access
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default TripInteractions;