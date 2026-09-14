"use client";

import { useState } from "react";
import styles from "./EditPopup.module.css";
import ErrorMessage from "@/components/ui/ErrorMessage";
import SuccessMessage from "@/components/ui/SuccessMessage";
import EditIcon from "@mui/icons-material/Edit";
import API from "@/utils/api";
import useAuthStore from "@/store/authStore";
import { Rating } from "@mui/material";

export default function WriteReview({ destinationId }) {
  const { roles } = useAuthStore();
  const [showPopup, setShowPopup] = useState(false);
  const [showProtectedPopup, setShowProtectedPopup] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [comment, setComment] = useState("");
  const [rate, setRate] = useState(0);

  const handleOpenPopup = () => {
    if (!roles.includes("User")) {
      setShowProtectedPopup(true);
      return;
    }
    setShowPopup(true);
    setComment("");
    setRate(0);
    setError("");
    setSuccess("");
  };

  const handleSubmit = async () => {
    if (!comment.trim()) {
      setError("Please write your review before submitting.");
      return;
    }

    if (rate === 0) {
      setError("Please select a rating before submitting.");
      return;
    }

    setLoading(true);
    setError("");
    setSuccess("");

    try {
      await API.post("/Review", {
        destinationId,
        comment: comment.trim(),
        rate,
      });

      setSuccess("Review submitted successfully!");
      setTimeout(() => {
        setShowPopup(false);
        setSuccess("");
      }, 2000);
    } catch (err) {
      console.error("Failed to submit review:", err);
      setError(
        err.response?.data?.message ||
          "Failed to submit review. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button className={styles.editButton} onClick={handleOpenPopup}>
        <EditIcon sx={{ fontSize: 22 }} /> Write a Review
      </button>

      {showPopup && (
        <div className={styles.popupOverlay}>
          <div className={styles.popupContent}>
            <button
              className={styles.closeButton}
              onClick={() => {
                setShowPopup(false);
              }}
            >
              ✕
            </button>

            <h2>Write a Review</h2>
            <p className={styles.reviewP}>
              Share your experience about this destination
            </p>

            <div className={styles.formContainer}>
              <div style={{ margin: "16px 0", textAlign: "center" }}>
                <Rating
                  name="half-rating"
                  value={rate}
                  precision={0.5}
                  onChange={(event, newValue) => {
                    setRate(newValue);
                  }}
                  size="large"
                  sx={{
                    "& .MuiRating-iconFilled": {
                      color: "var(--Green-Hover)",
                    },
                    "& .MuiRating-iconHover": {
                      color: "var(--Green-Hover)",
                    },
                  }}
                />
              </div>

              <textarea
                value={comment}
                onChange={(e) => setComment(e.target.value)}
                placeholder="Share your experience..."
                style={{
                  width: "100%",
                  minHeight: "150px",
                  padding: "12px",
                  borderRadius: "8px",
                  border: "1px solid var(--Neutrals-Light-Outline)",
                  fontSize: "16px",
                  resize: "vertical",
                  marginBottom: "8px",
                  fontFamily: "inherit",
                }}
              />

              {error && <ErrorMessage message={error} />}
              {success && <SuccessMessage message={success} />}

              <button
                type="button"
                className={styles.submitButton}
                onClick={handleSubmit}
                disabled={loading}
              >
                {loading ? "Submitting..." : "Submit Review"}
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
}
