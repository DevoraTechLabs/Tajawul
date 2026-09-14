"use client";

import styles from "@/forms.module.css";
import Input from "app/components/ui/Input";
import Textarea from "app/components/ui/Textarea";
import ImageUpload from "@/components/ui/ImageUpload";
import MultiDropdown from "@/components/ui/MultiDropdown";
import { RadioGroup, RadioGroupItem } from "app/components/ui/RadioGroup";
import { useState, useEffect, useCallback } from "react";
import { stepOneSchema } from "./actions";
import Dropdown from "app/components/ui/Dropdown";
import StepProgress from "@/components/ui/StepProgress";
import allCountriesStates from "@/data/allCountriesStates.json";
import languages from "@/data/languages.json";
import API from "@/utils/api";
import ErrorMessage from "app/components/ui/ErrorMessage";
import SuccessMessage from "app/components/ui/SuccessMessage";
import Image from "next/image";
import useAuthStore from "@/store/authStore";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { WhiteLoading } from "@/components/ui/Loading";

// Utility function for debouncing
const debounce = (func, delay) => {
  let timeout;
  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), delay);
  };
};

export default function StepOneForm() {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    username: "",
    bio: "",
    profilePicture: "",
    phoneNumber: "",
    birthDate: "",
    country: "",
    city: "",
    address: "",
    nationality: "",
    gender: "",
    spokenLanguages: [],
    maritalStatus: "",
    socialMediaLinks: [],
  });

  const [errors, setErrors] = useState({});
  const [cities, setCities] = useState([]);
  const [cityClicked, setCityClicked] = useState(false);
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [error, setError] = useState("");

  // Load saved form data from local storage
  useEffect(() => {
    const savedData = localStorage.getItem("stepOneForm");
    if (savedData) {
      const parsedData = JSON.parse(savedData);
      setFormData(parsedData);
    }
  }, []);

  // Save form data to local storage with debounce
  const saveToLocalStorage = useCallback(
    debounce((data) => {
      localStorage.setItem("stepOneForm", JSON.stringify(data));
    }, 500),
    []
  );

  useEffect(() => {
    saveToLocalStorage(formData);
  }, [formData, saveToLocalStorage]);

  // Debounced fetch cities function
  const fetchCities = useCallback(
    debounce((country) => {
      if (!country) {
        setCities([]);
        return;
      }

      const citiesForCountry = allCountriesStates[country] || [];
      setCities(citiesForCountry.map((city) => ({ value: city, label: city })));
    }, 500),
    []
  );

  useEffect(() => {
    fetchCities(formData.country);
  }, [formData.country, fetchCities]);

  // Handle Input Changes with Validation
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));

    try {
      stepOneSchema
        .pick({ [name]: stepOneSchema.shape[name] })
        .parse({ [name]: value });
      setErrors((prev) => ({ ...prev, [name]: null }));
    } catch (error) {
      setErrors((prev) => ({ ...prev, [name]: error.errors[0].message }));
    }
  };

  // Handle Gender Selection
  const handleGenderChange = (value) => {
    setFormData((prev) => ({ ...prev, gender: value }));
  };

  const handleFileUpload = (event) => {
    if (!event.target.files || event.target.files.length === 0) {
      setFormData((prev) => ({ ...prev, profilePicture: null }));
      return;
    }
    setFormData((prev) => ({ ...prev, profilePicture: event.target.files[0] }));
  };

  // Handle Spoken Language Selection
  const handleSpokenLanguagesChange = (selectedLanguages) => {
    setFormData((prev) => ({
      ...prev,
      spokenLanguages: selectedLanguages,
    }));
  };

  // Add social media link
  const addSocialMediaLink = () => {
    setFormData((prev) => ({
      ...prev,
      socialMediaLinks: [...prev.socialMediaLinks, { platform: "", url: "" }],
    }));
  };

  // Handle social media changes
  const handleSocialMediaChange = (index, field, value) => {
    const updatedLinks = [...formData.socialMediaLinks];
    updatedLinks[index] = { ...updatedLinks[index], [field]: value };
    setFormData((prev) => ({ ...prev, socialMediaLinks: updatedLinks }));

    if (field === "url") {
      let error = "";
      if (!/^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$/.test(value)) {
        error = "Invalid social media URL";
      }

      setErrors((prev) => ({ ...prev, [`socialMedia-${index}`]: error }));
    }
  };

  // Remove social media link
  const removeSocialMediaLink = (index) => {
    const updatedLinks = formData.socialMediaLinks.filter(
      (_, i) => i !== index
    );
    setFormData((prev) => ({ ...prev, socialMediaLinks: updatedLinks }));
  };

  // Handle Form Submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSuccess("");
    setError("");

    try {
      // First validate all form data except profile picture
      const { profilePicture, ...dataToSubmit } = formData;

      const validation = stepOneSchema.safeParse(dataToSubmit);
      if (!validation.success) {
        const newErrors = validation.error.format();
        setErrors(
          Object.keys(newErrors).reduce((acc, key) => {
            acc[key] = newErrors[key]?._errors?.[0] || "";
            return acc;
          }, {})
        );
        setLoading(false);
        return;
      }

      // Submit the main form data
      const formattedData = {
        username: dataToSubmit.username,
        firstName: dataToSubmit.firstName,
        lastName: dataToSubmit.lastName,
        bio: dataToSubmit.bio,
        phoneNumber: dataToSubmit.phoneNumber,
        birthDate: dataToSubmit.birthDate,
        city: dataToSubmit.city,
        country: dataToSubmit.country,
        nationality: dataToSubmit.nationality,
        gender: dataToSubmit.gender,
        spokenLanguages: dataToSubmit.spokenLanguages,
        maritalStatus: dataToSubmit.maritalStatus,
        socialMediaLinks: dataToSubmit.socialMediaLinks,
      };

      const response = await API.post("/User/info", formattedData);
      let imageUploadSuccess = true;

      // Upload image if it exists
      if (profilePicture) {
        try {
          const imageFormData = new FormData();
          imageFormData.append("ProfileImage", profilePicture);

          const imageResponse = await API.put(
            "/User/profile/image",
            imageFormData,
            {
              headers: {
                "Content-Type": "multipart/form-data",
              },
            }
          );

          if (imageResponse.status !== 200) {
            imageUploadSuccess = false;
            setError("Profile info saved, but image upload failed.");
          }
        } catch (imageError) {
          imageUploadSuccess = false;
          console.error("Image upload failed:", imageError);
          setError("Profile info saved, but image upload failed.");
        }
      }

      // Redirect if both requests succeeded
      // In the handleSubmit function, update the success cases:
      if (response.status === 200 && imageUploadSuccess) {
        // Replace the roles and tokens with the new ones from response
        useAuthStore.getState().replaceRoles(response.data.role);
        if (response.data.token && response.data.refreshToken) {
          useAuthStore
            .getState()
            .replaceTokens(response.data.token, response.data.refreshToken);
        }
        setSuccess("Profile info & image updated successfully. Redirecting...");
        router.push("/complete-your-profile/travel-interests");
      }

      // Also update the case where only profile info succeeded
      if (response.status === 200 && !imageUploadSuccess) {
        useAuthStore.getState().replaceRoles(response.data.role);
        if (response.data.token && response.data.refreshToken) {
          useAuthStore
            .getState()
            .replaceTokens(response.data.token, response.data.refreshToken);
        }
        setSuccess("Profile info updated, but image upload failed.");
      }
    } catch (err) {
      console.error("API Request Failed:", err.response?.data || err.message);
      setError(err.response?.data?.message || "Failed to update profile.");
    } finally {
      setLoading(false);
    }
  };

  // Prepare language options for MultiDropdown
  const languageOptions = languages.map((lang) => ({
    value: lang.English,
    label: lang.English,
  }));

  // Prepare country options for Nationality Dropdown
  const countryOptions = Object.keys(allCountriesStates).map((country) => ({
    value: country,
    label: country,
  }));

  const router = useRouter();
  const { roles } = useAuthStore();

  useEffect(() => {
    if (roles.includes("User")) {
      router.push("/complete-your-profile");
    }
  }, [roles, router]);

  // Show completion message if user has already completed this step
  if (roles.includes("Person") && roles.includes("CompletedSocialInfo")) {
    return (
      <div>
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
            href="/complete-your-profile/travel-interests"
            className={styles.ctaButton}
          >
            Continue to Next Step
          </Link>
        </div>
      </div>
    );
  }
  return (
    <div>
      <StepProgress />
      <p className={styles.title}>Complete Your Profile</p>
      <p className={styles.subheaderStepOne}>
        Step One: Tell us more about yourself
      </p>
      <p className={styles.descriptionStepOne}>
        <strong>Help us personalize your travel experiences. </strong> This also
        sets up your profile, unlocking social features to connect with fellow
        travelers who share your interests.
      </p>
      <form className={styles.formWidth} onSubmit={handleSubmit}>
        <div className={styles.formRow}>
          <Input
            label="First Name"
            id="firstName"
            type="text"
            required
            value={formData.firstName}
            onChange={handleChange}
            errorMsg={errors.firstName}
          />
          <Input
            label="Last Name"
            id="lastName"
            type="text"
            required
            value={formData.lastName}
            onChange={handleChange}
            errorMsg={errors.lastName}
          />
        </div>
        <ImageUpload
          label="Profile Picture"
          id="profilePicture"
          description="Accepted formats: jpg, jpeg, png, webp (max 2MB)"
          required
          onUpload={handleFileUpload}
          errorMsg={errors.profilePicture}
          accept="image/*"
        />
        <Input
          label="Username"
          id="username"
          type="text"
          required
          value={formData.username}
          onChange={handleChange}
          errorMsg={errors.username}
        />
        <Textarea
          label="Bio"
          id="bio"
          required
          value={formData.bio}
          onChange={handleChange}
          errorMsg={errors.bio}
        />

        {/* Social Media Links Section */}
        <div className={styles.contactButtons}>
          <button
            type="button"
            onClick={addSocialMediaLink}
            className={styles.addButton}
          >
            + Add Social Media
          </button>
        </div>
        {formData.socialMediaLinks.map((link, index) => (
          <div key={index} className={styles.contactInput}>
            <Dropdown
              label="Platform"
              id={`socialMediaPlatform-${index}`}
              value={link.platform}
              onChange={(e) =>
                handleSocialMediaChange(index, "platform", e.target.value)
              }
              options={[
                { value: "Facebook", label: "Facebook" },
                { value: "Instagram", label: "Instagram" },
                { value: "Twitter", label: "Twitter" },
                { value: "LinkedIn", label: "LinkedIn" },
              ]}
            />
            <Input
              label="URL"
              id={`socialMediaUrl-${index}`}
              type="url"
              value={link.url}
              onChange={(e) =>
                handleSocialMediaChange(index, "url", e.target.value)
              }
              errorMsg={errors[`socialMedia-${index}`]}
            />
            <button
              type="button"
              onClick={() => removeSocialMediaLink(index)}
              className={styles.removeButton}
            >
              Remove
            </button>
          </div>
        ))}
        <Input
          label="Phone Number"
          id="phoneNumber"
          description="Include country code if applicable."
          type="tel"
          required
          value={formData.phoneNumber}
          onChange={handleChange}
          errorMsg={errors.phoneNumber}
        />

        <Input
          label="Birth Date"
          id="birthDate"
          type="date"
          required
          value={formData.birthDate}
          onChange={handleChange}
          errorMsg={errors.birthDate}
        />
        <div className={styles.genderContainer}>
          <RadioGroup
            label="Gender"
            value={formData.gender}
            onValueChange={handleGenderChange}
            required={true}
          >
            <RadioGroupItem value="male" label="Male" />
            <RadioGroupItem value="female" label="Female" />
          </RadioGroup>
        </div>
        <Dropdown
          label="Marital Status"
          id="maritalStatus"
          required
          value={formData.maritalStatus}
          onChange={handleChange}
          options={[
            { value: "Single", label: "Single" },
            { value: "Married", label: "Married" },
          ]}
          errorMsg={errors.maritalStatus}
        />
        <Dropdown
          label="Nationality"
          id="nationality"
          required
          value={formData.nationality}
          onChange={handleChange}
          options={countryOptions}
          errorMsg={errors.nationality}
        />
        <div className={styles.formRow}>
          <Dropdown
            label="Country"
            id="country"
            required
            value={formData.country}
            onChange={handleChange}
            options={countryOptions}
            errorMsg={errors.country}
          />
          <Dropdown
            label="City"
            id="city"
            required
            value={formData.city}
            onChange={handleChange}
            options={cities}
            errorMsg={
              cityClicked && !formData.country
                ? "Please Select a Country"
                : errors.city
            }
            disabled={!formData.country || cities.length === 0}
            onDropdownClick={() => setCityClicked(true)}
          />
        </div>
        <Input
          label="Address"
          id="address"
          type="text"
          required
          value={formData.address}
          onChange={handleChange}
          errorMsg={errors.address}
        />
        <MultiDropdown
          label="Spoken Language/s"
          id="spokenLanguages"
          required
          options={languageOptions}
          value={formData.spokenLanguages}
          onChange={(e) => handleSpokenLanguagesChange(e.target.value)}
        />

        {success && <SuccessMessage message={success} />}
        {error && <ErrorMessage message={error} />}

        <button
          type="submit"
          className={styles.submitButton}
          disabled={loading}
        >
          {loading ? <WhiteLoading /> : "Next Step"}
        </button>
      </form>
    </div>
  );
}
