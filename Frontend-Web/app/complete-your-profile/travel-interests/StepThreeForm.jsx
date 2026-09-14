import styles from "@/forms.module.css";
import React, { useState, useEffect } from "react";
import InterestsTagQuestion from "@/components/ui/InterestsTagQuestion";
import { groupSizeIcons } from "@/utils/tagIconsMapping";
import axios from "axios";
import ErrorMessage from "app/components/ui/ErrorMessage";
import SuccessMessage from "app/components/ui/SuccessMessage";
import API from "@/utils/api";
import StepProgress from "@/components/ui/StepProgress";
import Image from "next/image";
import useAuthStore from "@/store/authStore";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { GreenLoading } from "@/components/ui/Loading";
import { WhiteLoading } from "@/components/ui/Loading";
import Divider from "@mui/material/Divider";

// Import local JSON files
import tagsData from "@/data/tags.json";
import destinationTypesData from "@/data/destinationTypes.json";
import activitiesData from "@/data/activities.json";

const durationOptions = [
  { label: "Day", value: "Day" },
  { label: "3 Days", value: "3 Days" },
  { label: "Week", value: "Week" },
  { label: "2 Weeks", value: "2 Weeks" },
  { label: "Month", value: "Month" },
  { label: "More than a month", value: "More than Month" },
];

const budgetOptions = [
  { label: "$ Low", value: "low" },
  { label: "$$ Mid-range", value: "mid" },
  { label: "$$$ Luxury", value: "luxury" },
];

const groupSizeOptions = [
  { label: "Solo", value: "solo", icon: groupSizeIcons.solo },
  { label: "Couple", value: "couple", icon: groupSizeIcons.couple },
  { label: "Family", value: "family", icon: groupSizeIcons.family },
  { label: "Group", value: "group", icon: groupSizeIcons.group },
  {
    label: "Big Group",
    value: "big group",
    icon: groupSizeIcons["big-group"],
  },
];

// Helper function to process JSON data
const processJsonData = (data) => {
  // If data is an array, return it directly
  if (Array.isArray(data)) return data;
  // If data has an 'activities' property that's an array (like your example)
  if (data.activities && Array.isArray(data.activities)) return data.activities;
  if (data.tags && Array.isArray(data.tags)) return data.tags;
  if (data.destinations && Array.isArray(data.destinations))
    return data.destinations;
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

export default function StepThreeForm() {
  const [selectedTags, setSelectedTags] = useState({
    tags: [],
    destinationTypes: [],
    activities: [],
    tripDurations: [],
    priceRanges: [],
    groupSizes: [],
  });
  const [isLoading, setIsLoading] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [loadError, setLoadError] = useState(null);
  const [submitError, setSubmitError] = useState(null);
  const [success, setSuccess] = useState("");

  const router = useRouter();
  const { roles, addRole } = useAuthStore();

  useEffect(() => {
    if (roles.includes("User")) {
      router.push("/complete-your-profile");
    }
  }, [roles, router]);

  // Process the JSON data on component mount
  useEffect(() => {
    try {
      // Process each JSON file according to its structure
      const tagsOptions = convertToOptions(processJsonData(tagsData));
      const destinationTypesOptions = convertToOptions(
        processJsonData(destinationTypesData)
      );
      const activitiesOptions = convertToOptions(
        processJsonData(activitiesData)
      );

      // Store the processed options in state
      setSelectedTags((prev) => ({
        ...prev,
        tagsOptions,
        destinationTypesOptions,
        activitiesOptions,
      }));
    } catch (err) {
      console.error("Failed to process options:", err);
      setLoadError(err);
    }
  }, []);

  const handleTagChange = (field) => (selectedValues) => {
    setSelectedTags((prev) => ({
      ...prev,
      [field]: selectedValues,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitLoading(true);
    setSubmitError(null);
    setSuccess("");

    try {
      const requestBody = {
        destinationTypes: selectedTags.destinationTypes,
        groupSizes: selectedTags.groupSizes,
        activities: selectedTags.activities,
        tags: selectedTags.tags,
        tripDurations: selectedTags.tripDurations,
        priceRanges: selectedTags.priceRanges,
      };

      const response = await API.post("/User/interests", requestBody);

      if (response.data.role) {
        useAuthStore.getState().replaceRoles(response.data.role);
        if (response.data.token && response.data.refreshToken) {
          useAuthStore
            .getState()
            .replaceTokens(response.data.token, response.data.refreshToken);
        }
      }

      setSuccess(
        response.data.message ||
          "Travel preferences saved successfully! Redirecting..."
      );

      setTimeout(() => {
        router.push("/complete-your-profile");
      }, 2000);
    } catch (err) {
      console.error("API Request Failed:", err.response?.data || err.message);
      setSubmitError(
        err.response?.data?.message || "Failed to save travel preferences."
      );
    } finally {
      setSubmitLoading(false);
    }
  };

  if (roles.includes("Person") && roles.includes("CompletedInterestInfo")) {
    return (
      <div className={styles.container}>
        <StepProgress />
        <div className={styles.completedStep}>
          <Image
            src="/one-step-completed.svg"
            alt="Step completed"
            width={350}
            height={350}
            className={styles.completedImage}
          />
          <p className={styles.completedText}>
            You've already completed this step, only one step is left
          </p>
          <Link
            href="/complete-your-profile/user-info"
            className={styles.ctaButton}
          >
            Continue to Next Step
          </Link>
        </div>
      </div>
    );
  }

  if (loadError && !submitLoading) {
    return <div>Error loading options. Please try again later.</div>;
  }

  return (
    <div>
      <StepProgress />
      <p className={styles.title}>Complete Your Profile</p>
      <p className={styles.subheaderStepTwo}>
        Step Two: Tell Us About Your Travel Preferences
      </p>
      <p className={styles.descriptionStepTwo}>
        <strong>Choose All that may apply, </strong>Help us understand your
        travel style so we can recommend the best destinations and experiences
        for you.
      </p>

      <form onSubmit={handleSubmit}>
        <InterestsTagQuestion
          question="What types of travel experiences excite you the most?"
          description="Select 1-10 options that best match your travel preferences"
          options={convertToOptions(processJsonData(tagsData))}
          selectedValues={selectedTags.tags}
          onChange={handleTagChange("tags")}
          required
          maxSelections={10}
        />
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Neautrals-Medium-Outline)",
            marginBottom: "20px",
          }}
        />
        <InterestsTagQuestion
          question="Which destinations are you most interested in?"
          description="Choose 1-10 destination types you'd like to visit"
          options={convertToOptions(processJsonData(destinationTypesData))}
          selectedValues={selectedTags.destinationTypes}
          onChange={handleTagChange("destinationTypes")}
          required
          maxSelections={10}
        />
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Neautrals-Medium-Outline)",
            marginBottom: "20px",
          }}
        />
        <InterestsTagQuestion
          question="What activities do you enjoy while traveling?"
          description="Pick 1-10 activities that make your trips memorable"
          options={convertToOptions(processJsonData(activitiesData))}
          selectedValues={selectedTags.activities}
          onChange={handleTagChange("activities")}
          required
          maxSelections={10}
        />
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Neautrals-Medium-Outline)",
            marginBottom: "20px",
          }}
        />
        <InterestsTagQuestion
          question="How long do your trips usually last?"
          description="Select 1-3 typical durations for your trips"
          options={durationOptions}
          selectedValues={selectedTags.tripDurations}
          onChange={handleTagChange("tripDurations")}
          required
          maxSelections={3}
        />
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Neautrals-Medium-Outline)",
            marginBottom: "20px",
          }}
        />
        <InterestsTagQuestion
          question="What's your usual travel budget?"
          description="Select one price range that fits your typical spending"
          options={budgetOptions}
          selectedValues={selectedTags.priceRanges}
          onChange={handleTagChange("priceRanges")}
          required
          maxSelections={1}
        />
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Neautrals-Medium-Outline)",
            marginBottom: "20px",
          }}
        />
        <InterestsTagQuestion
          question="Who do you usually travel with?"
          description="Choose 1-2 options that describe your travel companions"
          options={groupSizeOptions}
          selectedValues={selectedTags.groupSizes}
          onChange={handleTagChange("groupSizes")}
          required
          maxSelections={2}
        />

        {success && <SuccessMessage message={success} />}
        {submitError && <ErrorMessage message={submitError} />}

        <button
          type="submit"
          className={styles.submitButton}
          disabled={submitLoading}
        >
          {submitLoading ? <WhiteLoading /> : "Save Preferences"}
        </button>
      </form>
    </div>
  );
}
