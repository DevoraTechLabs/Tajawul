"use client";

import { useState, useEffect } from "react";
import styles from "./EditPopup.module.css";
import EditableTagQuestion from "../EditableTagQuestion";
import ErrorMessage from "@/components/ui/ErrorMessage";
import SuccessMessage from "@/components/ui/SuccessMessage";
import API from "@/utils/api";
import axios from "axios";
import EditIcon from "@mui/icons-material/Edit";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { groupSizeIcons } from "@/utils/tagIconsMapping";
import Image from "next/image";
import useAuthStore from "@/store/authStore";

// Import local JSON files
import tagsData from "@/data/tags.json";
import activitiesData from "@/data/activities.json";

// Helper function to process JSON data
const processJsonData = (data) => {
  // If data is an array, return it directly
  if (Array.isArray(data)) return data;
  // If data has an 'activities' property that's an array
  if (data.activities && Array.isArray(data.activities)) return data.activities;
  if (data.tags && Array.isArray(data.tags)) return data.tags;
  // Fallback to empty array
  return [];
};

// Convert string array to options format, sorted alphabetically
const convertToOptions = (items) => {
  return items
    .slice() // copy to avoid mutating original
    .sort((a, b) => a.localeCompare(b))
    .map((item) => ({
      label: item,
      value: item,
    }));
};

export default function EditTags({ destinationId }) {
  const { roles } = useAuthStore();
  const [showPopup, setShowPopup] = useState(false);
  const [showProtectedPopup, setShowProtectedPopup] = useState(false);
  const [currentStep, setCurrentStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [fetchingData, setFetchingData] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [groupSize, setGroupSize] = useState([]);
  const [tags, setTags] = useState([]);
  const [activities, setActivities] = useState([]);
  const [deletedGroupSizes, setDeletedGroupSizes] = useState([]);
  const [deletedTags, setDeletedTags] = useState([]);
  const [deletedActivities, setDeletedActivities] = useState([]);
  const [tagsOptions, setTagsOptions] = useState([]);
  const [activitiesOptions, setActivitiesOptions] = useState([]);

  // Process the JSON data on component mount
  useEffect(() => {
    try {
      // Process each JSON file according to its structure
      const tagsOptions = convertToOptions(processJsonData(tagsData));
      const activitiesOptions = convertToOptions(
        processJsonData(activitiesData)
      );

      // Store the processed options in state
      setTagsOptions(tagsOptions);
      setActivitiesOptions(activitiesOptions);
    } catch (err) {
      console.error("Failed to process options:", err);
      setError("Failed to load tag options. Please try again later.");
    }
  }, []);

  const handleOpenPopup = async () => {
    if (!roles.includes("User")) {
      setShowProtectedPopup(true);
      return;
    }

    setFetchingData(true);
    setError("");
    try {
      const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
      const response = await axios.get(
        `${baseUrl}/Destination/${destinationId}/attributes`
      );
      const data = response.data;

      setGroupSize(data.groupSizes?.data?.map((item) => item.group) || []);
      setTags(data.tags?.data?.map((item) => item.name) || []);
      setActivities(data.activities?.data?.map((item) => item.name) || []);

      setDeletedGroupSizes([]);
      setDeletedTags([]);
      setDeletedActivities([]);

      setShowPopup(true);
    } catch (err) {
      console.error("Failed to fetch attributes:", err);
      setError("Failed to fetch existing tags. Please try again.");
    } finally {
      setFetchingData(false);
    }
  };

  const groupSizeOptions = [
    { label: "Solo", value: "Solo", icon: groupSizeIcons.solo },
    { label: "Couple", value: "Couple", icon: groupSizeIcons.couple },
    { label: "Family", value: "Family", icon: groupSizeIcons.family },
    { label: "Group", value: "Group", icon: groupSizeIcons.group },
    {
      label: "Big Group",
      value: "Big Group",
      icon: groupSizeIcons["big-group"],
    },
  ];

  const handleGroupSizeChange = (e) => {
    setGroupSize(e.target.value);
  };

  const handleGroupSizeDelete = (e) => {
    setDeletedGroupSizes(e.target.value);
  };

  const handleTagsChange = (e) => {
    // Filter out any values that aren't in our options (new tags are handled by onAddNew)
    const validValues = e.target.value.filter((val) =>
      tagsOptions.some((opt) => opt.value === val)
    );
    setTags(validValues);
  };

  const handleTagsDelete = (e) => {
    setDeletedTags(e.target.value);
  };

  const handleActivitiesChange = (e) => {
    // Filter out any values that aren't in our options (new activities are handled by onAddNew)
    const validValues = e.target.value.filter((val) =>
      activitiesOptions.some((opt) => opt.value === val)
    );
    setActivities(validValues);
  };

  const handleActivitiesDelete = (e) => {
    setDeletedActivities(e.target.value);
  };

  const handleNext = () => {
    setCurrentStep(2);
  };

  const handleBack = () => {
    setCurrentStep(1);
  };

  const handleAddNewTag = (newTag) => {
    setTags((prev) => [...prev, newTag.value]);
    // Also update the options to include the new tag
    setTagsOptions((prev) => [
      ...prev,
      { label: newTag.value, value: newTag.value },
    ]);
  };

  const handleAddNewActivity = (newActivity) => {
    setActivities((prev) => [...prev, newActivity.value]);
    // Also update the options to include the new activity
    setActivitiesOptions((prev) => [
      ...prev,
      { label: newActivity.value, value: newActivity.value },
    ]);
  };

  const handleSubmit = async () => {
    setLoading(true);
    setError("");
    setSuccess("");

    try {
      if (
        deletedGroupSizes.length > 0 ||
        deletedTags.length > 0 ||
        deletedActivities.length > 0
      ) {
        await API.delete(`/Destination/${destinationId}/attributes`, {
          data: {
            groupSizes: deletedGroupSizes,
            tags: deletedTags,
            activities: deletedActivities,
          },
        });
      }

      await API.post(`/Destination/${destinationId}/attributes`, {
        groupSizes: groupSize,
        tags: tags,
        activities: activities,
      });

      setSuccess("Tags updated successfully!");
      setTimeout(() => {
        setShowPopup(false);
        setSuccess("");
        setCurrentStep(1);
      }, 2000);
    } catch (err) {
      console.error("Failed to update tags:", err);
      setError(err.response?.data?.message || "Failed to update tags.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button
        className={styles.editButton}
        onClick={handleOpenPopup}
        disabled={fetchingData}
      >
        {fetchingData ? (
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
                setCurrentStep(1);
              }}
            >
              ✕
            </button>

            <h2>Add/Edit Destination Tags</h2>
            <p>Choose all that best describes this destination</p>

            <div className={styles.formContainer}>
              {currentStep === 1 ? (
                <>
                  <EditableTagQuestion
                    question="Tags"
                    options={tagsOptions}
                    selectedValues={tags}
                    onChange={handleTagsChange}
                    onDelete={handleTagsDelete}
                    size="small"
                    addNewText="+ Add Tag"
                    onAddNew={handleAddNewTag}
                  />
                </>
              ) : (
                <>
                  <button
                    className={styles.backButton}
                    onClick={handleBack}
                    type="button"
                  >
                    <ArrowBackIcon sx={{ fontSize: 18 }} /> Back
                  </button>
                  <EditableTagQuestion
                    question="Group Size"
                    options={groupSizeOptions}
                    selectedValues={groupSize}
                    onChange={handleGroupSizeChange}
                    onDelete={handleGroupSizeDelete}
                    size="small"
                  />
                  <EditableTagQuestion
                    question="Activities"
                    options={activitiesOptions}
                    selectedValues={activities}
                    onChange={handleActivitiesChange}
                    onDelete={handleActivitiesDelete}
                    size="small"
                    addNewText="+ Add Activity"
                    onAddNew={handleAddNewActivity}
                  />
                </>
              )}

              {error && <ErrorMessage message={error} />}
              {success && <SuccessMessage message={success} />}

              <button
                type="button"
                className={styles.submitButton}
                onClick={currentStep === 1 ? handleNext : handleSubmit}
                disabled={loading}
              >
                {loading
                  ? "Saving..."
                  : currentStep === 1
                  ? "Next"
                  : "Save Tags"}
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
