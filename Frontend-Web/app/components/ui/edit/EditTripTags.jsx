"use client";

import { useState, useEffect } from "react";
import styles from "./EditPopup.module.css";
import EditableTagQuestion from "../EditableTagQuestion";
import ErrorMessage from "@/components/ui/ErrorMessage";
import SuccessMessage from "@/components/ui/SuccessMessage";
import API from "@/utils/api";
import axios from "axios";
import EditIcon from "@mui/icons-material/Edit";

export default function EditTripTags({ tripId }) {
  const [showPopup, setShowPopup] = useState(false);
  const [loading, setLoading] = useState(false);
  const [fetchingData, setFetchingData] = useState(false);
  const [fetchingOptions, setFetchingOptions] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [tags, setTags] = useState([]);
  const [deletedTags, setDeletedTags] = useState([]);
  const [tagsOptions, setTagsOptions] = useState([]);

  // Fetch tag options when component mounts
  useEffect(() => {
    const fetchOptions = async () => {
      setFetchingOptions(true);
      try {
        const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
        const tagsResponse = await axios.get(`${baseUrl}/trip-tags`); // Changed endpoint

        const processOptions = (items) => {
          return (
            items?.map((item) => ({
              label: item.name,
              value: item.name,
            })) || []
          );
        };

        setTagsOptions(processOptions(tagsResponse.data));
      } catch (err) {
        console.error("Failed to fetch tag options:", err);
        setError("Failed to load tag options. Please try again later.");
      } finally {
        setFetchingOptions(false);
      }
    };

    fetchOptions();
  }, []);

  const handleOpenPopup = async () => {
    setFetchingData(true);
    setError("");
    try {
      const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
      const response = await axios.get(
        `${baseUrl}/Trip/${tripId}/tags` // Changed endpoint
      );
      const data = response.data;

      setTags(data.tags?.map((item) => item.name) || []); // Adjusted data structure
      setDeletedTags([]);
      setShowPopup(true);
    } catch (err) {
      console.error("Failed to fetch tags:", err);
      setError("Failed to fetch existing tags. Please try again.");
    } finally {
      setFetchingData(false);
    }
  };

  const handleTagsChange = (e) => {
    setTags(e.target.value);
  };

  const handleTagsDelete = (e) => {
    setDeletedTags(e.target.value);
  };

  const handleSubmit = async () => {
    setLoading(true);
    setError("");
    setSuccess("");

    try {
      // First delete removed tags if any
      if (deletedTags.length > 0) {
        await API.delete(`/Trip/${tripId}/tags`, { // Changed endpoint
          data: {
            tags: deletedTags,
          },
        });
      }

      // Then add/update remaining tags
      await API.post(`/Trip/${tripId}/tags`, { // Changed endpoint
        tags: tags,
      });

      setSuccess("Trip tags updated successfully!"); // Updated success message
      setTimeout(() => {
        setShowPopup(false);
        setSuccess("");
      }, 2000);
    } catch (err) {
      console.error("Failed to update tags:", err);
      setError(err.response?.data?.message || "Failed to update trip tags.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button
        className={styles.editButton}
        onClick={handleOpenPopup}
        disabled={fetchingData || fetchingOptions}
      >
        {fetchingData || fetchingOptions ? (
          "Loading..."
        ) : (
          <>
            <EditIcon sx={{ fontSize: 22 }} /> Edit Tags
          </>
        )}
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

            <h2>Edit Trip Tags</h2> {/* Updated title */}
            <p>Choose tags that best describe this trip</p> 

            <div className={styles.formContainer}>
              <EditableTagQuestion
                question="Trip Tags" 
                options={tagsOptions}
                selectedValues={tags}
                onChange={handleTagsChange}
                onDelete={handleTagsDelete}
                size="small"
              />

              {error && <ErrorMessage message={error} />}
              {success && <SuccessMessage message={success} />}

              <button
                type="button"
                className={styles.submitButton}
                onClick={handleSubmit}
                disabled={loading || fetchingOptions}
              >
                {loading ? "Saving..." : "Save Tags"}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}