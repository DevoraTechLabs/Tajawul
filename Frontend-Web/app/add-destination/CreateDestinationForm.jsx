"use client";

import styles from "@/forms.module.css";
import Input from "app/components/ui/Input";
import Textarea from "app/components/ui/Textarea";
import Dropdown from "app/components/ui/Dropdown";
import ErrorMessage from "app/components/ui/ErrorMessage";
import SuccessMessage from "app/components/ui/SuccessMessage";
import Divider from "@mui/material/Divider";
import { useState, useEffect, useCallback } from "react";
import { addDestinationSchema, validateOpenCloseTime } from "./actions";
import DOMPurify from "dompurify";
import useAuthStore from "@/store/authStore";
import arabCountries from "@/data/arabCountries.json";
import API from "@/utils/api";
import { useRouter } from "next/navigation";
import destinationTypes from "@/data/destinationTypes.json";
import Image from "next/image";
import Link from "next/link";

import dynamic from "next/dynamic";

const LocationPicker = dynamic(
  () => import("@/components/map/LocationPicker"),
  {
    ssr: false,
    loading: () => <p>Loading map...</p>,
  }
);

// Debounce utility function to limit the number of calls to saveToLocalStorage
const debounce = (func, delay) => {
  let timeout;
  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), delay);
  };
};

// Reusable component for dynamic inputs
const DynamicInput = ({ label, type, value, onChange, errorMsg, onRemove }) => (
  <div className={styles.contactInput}>
    <Input
      label={label}
      type={type}
      value={value}
      onChange={onChange}
      errorMsg={errorMsg}
    />
    <button
      type="button"
      onClick={onRemove}
      className={styles.removeButton}
      aria-label={`Remove ${label}`}
    >
      Remove
    </button>
  </div>
);

export default function CreateDestinationForm() {
  const [formData, setFormData] = useState({
    name: "",
    type: "",
    description: "",
    priceRange: "",
    country: "",
    city: "",
    locations: [{ longitude: 0, latitude: 0, address: "" }],
    isOpen24Hours: false,
    openTime: "",
    closeTime: "",
    establishedAt: "",
    socialMediaLinks: [],
    contactInfo: [],
  });

  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [error, setError] = useState("");
  const [cities, setCities] = useState([]);
  const [cityClicked, setCityClicked] = useState(false);
  const { roles } = useAuthStore();
  const router = useRouter();

  const arabCountriesOptions = Object.keys(arabCountries).map((country) => ({
    value: country,
    label: country,
  }));

  // Show protected feature message if user doesn't have the right role
  if (!roles.includes("User")) {
    return (
      <div>
        <div className={styles.completedStep}>
          <Image
            src="/protected-feature.svg"
            alt="Protected feature"
            width={580}
            height={400}
            className={styles.completedImage}
          />

          <Link href="/complete-your-profile" className={styles.ctaButton}>
            Complete Your Profile to Access
          </Link>
        </div>
      </div>
    );
  }

  // Debounced fetch cities function
  const fetchCities = useCallback(
    debounce((country) => {
      if (!country) {
        setCities([]);
        return;
      }

      const citiesForCountry = arabCountries[country] || [];
      setCities(citiesForCountry.map((city) => ({ value: city, label: city })));
    }, 500),
    []
  );

  useEffect(() => {
    fetchCities(formData.country);
  }, [formData.country, fetchCities]);

  // Load saved form data from local storage
  useEffect(() => {
    const savedData = localStorage.getItem("createDestinationForm");
    if (savedData) {
      setFormData(JSON.parse(savedData));
    }
  }, []);

  // Save form data to local storage
  const saveToLocalStorage = useCallback(
    debounce((data) => {
      // Save all fields, including country and city
      localStorage.setItem("createDestinationForm", JSON.stringify(data));
    }, 500),
    []
  );

  useEffect(() => {
    saveToLocalStorage(formData);
  }, [formData, saveToLocalStorage]);

  const handleChange = (e) => {
    const { name, value } = e.target;

    // Sanitize the description field
    const sanitizedValue =
      name === "description" ? DOMPurify.sanitize(value) : value;

    setFormData((prev) => ({
      ...prev,
      [name]: sanitizedValue,
      ...(name === "country" ? { city: "" } : {}),
    }));

    try {
      if (
        sanitizedValue ||
        addDestinationSchema.shape[name]?._def?.isOptional !== true
      ) {
        addDestinationSchema
          .pick({ [name]: addDestinationSchema.shape[name] })
          .parse({ [name]: sanitizedValue });
      }
      setErrors((prev) => ({ ...prev, [name]: null }));
    } catch (error) {
      setErrors((prev) => ({ ...prev, [name]: error.errors[0].message }));
    }
  };

  const handleCheckboxChange = (e) => {
    setFormData((prev) => ({
      ...prev,
      isOpen24Hours: e.target.checked,
      openTime: e.target.checked ? "" : prev.openTime,
      closeTime: e.target.checked ? "" : prev.closeTime,
    }));
  };

  const addContactInfo = (type) => {
    setFormData((prev) => ({
      ...prev,
      contactInfo: [...prev.contactInfo, { type, value: "" }],
    }));
  };

  const addSocialMediaLink = () => {
    setFormData((prev) => ({
      ...prev,
      socialMediaLinks: [...prev.socialMediaLinks, { platform: "", url: "" }],
    }));
  };

  const handleContactChange = (index, value) => {
    const updatedContacts = [...formData.contactInfo];
    updatedContacts[index].value = value;
    setFormData((prev) => ({ ...prev, contactInfo: updatedContacts }));

    let error = "";
    if (updatedContacts[index].type === "phone") {
      if (!/^\+?\d{1,4}[\s\d]{6,15}$/.test(value)) {
        error = "Invalid phone number format";
      }
    } else if (updatedContacts[index].type === "website") {
      if (!/^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$/.test(value)) {
        error = "Invalid website URL";
      }
    }

    setErrors((prev) => ({ ...prev, [`contactInfo-${index}`]: error }));
  };

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

  const handleLocationChange = (field, value) => {
    setFormData((prev) => ({
      ...prev,
      locations: prev.locations.length
        ? [{ ...prev.locations[0], [field]: value }]
        : [{ longitude: 0, latitude: 0, address: "", [field]: value }],
    }));
  };

  const removeContactInfo = (index) => {
    const updatedContacts = formData.contactInfo.filter((_, i) => i !== index);
    setFormData((prev) => ({ ...prev, contactInfo: updatedContacts }));
  };

  const removeSocialMediaLink = (index) => {
    const updatedLinks = formData.socialMediaLinks.filter(
      (_, i) => i !== index
    );
    setFormData((prev) => ({ ...prev, socialMediaLinks: updatedLinks }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSuccess("");
    setError("");

    try {
      validateOpenCloseTime(formData);

      // Sanitize the description before submission
      const sanitizedDescription = DOMPurify.sanitize(formData.description);

      // Create a base object with required fields
      const formattedData = {
        name: formData.name,
        type: formData.type,
        description: sanitizedDescription,
        priceRange: formData.priceRange,
        country: formData.country,
        city: formData.city,
        locations: formData.locations.map((loc) => ({
          longitude: parseFloat(loc.longitude),
          latitude: parseFloat(loc.latitude),
          address: loc.address,
        })),
        isOpen24Hours: formData.isOpen24Hours,
      };

      // Add openTime and closeTime only if isOpen24Hours is false
      if (!formData.isOpen24Hours) {
        formattedData.openTime = formData.openTime;
        formattedData.closeTime = formData.closeTime;
      }

      // Add optional fields only if they have a value
      if (formData.establishedAt) {
        // Trim the establishedAt value before using it
        formattedData.establishedAt = `${formData.establishedAt.trim()}-01-01`;
      }

      if (formData.socialMediaLinks.length > 0) {
        formattedData.socialMediaLinks = formData.socialMediaLinks;
      }

      if (formData.contactInfo.length > 0) {
        formattedData.contactInfo = formData.contactInfo;
      }

      console.log("Submitted Data:", formattedData); // Log the submitted data

      const validation = addDestinationSchema.safeParse(formattedData);
      if (!validation.success) {
        const newErrors = validation.error.format();

        // Log the errors to the console
        console.log("Validation Errors:", newErrors);

        setErrors(
          Object.keys(newErrors).reduce((acc, key) => {
            acc[key] = newErrors[key]?._errors?.[0] || "";
            return acc;
          }, {})
        );
        setLoading(false);
        return;
      }

      // Retrieve the accessToken from the auth store
      const { accessToken } = useAuthStore.getState();

      console.log("Retrieved accessToken:", accessToken); // Debug: Log the retrieved token

      if (!accessToken) {
        console.error("No access token available. Please log in."); // Debug: Log if no token is found
        throw new Error("No access token available. Please log in.");
      }

      // Include the accessToken in the headers
      console.log("Including accessToken in headers:", accessToken); // Debug: Log the token being included in headers

      const response = await API.post("Destination", formattedData);

      console.log("API Response:", response.data); // Debug: Log the API response

      setSuccess(response.data.message || "Destination created successfully!");

      // Store the destination ID in local storage
      const destinationId = response.data.destination.destinationId; // Correctly access the destinationId
      localStorage.setItem("destinationId", destinationId);

      // Redirect to the images-upload page
      router.push("/add-destination/images-upload");

      // Reset form fields
      setFormData({
        name: "",
        type: "",
        description: "",
        country: "",
        city: "",
        openTime: "",
        closeTime: "",
        priceRange: "",
        contactInfo: [],
        socialMediaLinks: [],
        establishedAt: "",
        locations: [{ longitude: 0, latitude: 0, address: "" }],
        isOpen24Hours: false,
      });

      localStorage.removeItem("createDestinationForm");
    } catch (err) {
      console.error("API Request Failed:", err.response?.data || err.message);
      setError(err.response?.data?.message || "Failed to create destination.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.formContainer}>
      <p className={styles.title}>Add Destination</p>
      <form className={styles.formWidth} onSubmit={handleSubmit}>
        <h2 className={styles.subheader}>General Info</h2>
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Green-Perfect)",
            marginBottom: "20px",
          }}
        />
        <Input
          label="Destination Name"
          id="name"
          type="text"
          required
          value={formData.name}
          onChange={handleChange}
          errorMsg={errors.name}
        />

        <Textarea
          label="Description"
          id="description"
          required
          value={formData.description}
          onChange={handleChange}
          errorMsg={errors.description}
        />
        <div className={styles.formRow}>
          <Dropdown
            label="Destination Type"
            id="type"
            required
            value={formData.type}
            onChange={handleChange}
            options={destinationTypes.destinations.map((type) => ({
              value: type.toLowerCase(),
              label: type,
            }))}
            errorMsg={errors.type}
          />
          <Input
            label="Establishment Year"
            id="establishedAt"
            type="text"
            value={formData.establishedAt}
            onChange={handleChange}
            errorMsg={errors.establishedAt}
          />
        </div>

        <h2 className={styles.subheader}>Location Details</h2>
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Green-Perfect)",
            marginBottom: "20px",
          }}
        />
        <div className={styles.formRow}>
          <Dropdown
            label="Country"
            id="country"
            required
            value={formData.country}
            onChange={handleChange}
            options={arabCountriesOptions}
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
        <div className={styles.formRow}>
          <Input
            label="Address"
            id="address"
            type="text"
            required
            value={formData.locations[0]?.address || ""}
            onChange={(e) =>
              setFormData((prev) => ({
                ...prev,
                locations: [{ ...prev.locations[0], address: e.target.value }],
              }))
            }
            errorMsg={errors.address}
          />
        </div>
        <div className={styles.coordinatesContainer}>
          <div className={styles.longitudeField}>
            <Input
              label="Latitude"
              id="latitude"
              type="number"
              step="any"
              required
              value={formData.locations[0]?.latitude || 0}
              onChange={(e) => {
                setFormData((prev) => ({
                  ...prev,
                  locations: [
                    { ...prev.locations[0], latitude: e.target.value },
                  ],
                }));
              }}
              errorMsg={errors.latitude}
            />
          </div>

          <div className={styles.latitudeField}>
            <Input
              label="Longitude"
              id="longitude"
              type="number"
              step="any"
              required
              value={formData.locations[0]?.longitude || 0}
              onChange={(e) => {
                setFormData((prev) => ({
                  ...prev,
                  locations: [
                    { ...prev.locations[0], longitude: e.target.value },
                  ],
                }));
              }}
              errorMsg={errors.longitude}
            />
            <LocationPicker
              onLocationSelect={(location) => {
                setFormData((prev) => ({
                  ...prev,
                  locations: [
                    {
                      ...prev.locations[0],
                      longitude: location.lng,
                      latitude: location.lat,
                      address:
                        location.address || prev.locations[0]?.address || "",
                    },
                  ],
                }));
              }}
              initialLocation={{
                lng: formData.locations[0]?.longitude || 0,
                lat: formData.locations[0]?.latitude || 0,
                address: formData.locations[0]?.address || "",
              }}
            />
          </div>
        </div>

        <h2 className={styles.subheader}>Operating Hours & Pricing</h2>
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Green-Perfect)",
            marginBottom: "20px",
          }}
        />
        <div className={styles.checkboxContainer}>
          <input
            type="checkbox"
            id="isOpen24Hours"
            checked={formData.isOpen24Hours}
            onChange={handleCheckboxChange}
          />
          <label htmlFor="isOpen24Hours">Open 24 Hours</label>
        </div>
        {!formData.isOpen24Hours && (
          <div className={styles.formRow}>
            <Input
              label="Open Time"
              id="openTime"
              type="time"
              required
              value={formData.openTime}
              onChange={handleChange}
              errorMsg={errors.openTime}
            />
            <Input
              label="Close Time"
              id="closeTime"
              type="time"
              required
              value={formData.closeTime}
              onChange={handleChange}
              errorMsg={errors.closeTime}
            />
          </div>
        )}
        <Dropdown
          label="Price Range"
          id="priceRange"
          required
          value={formData.priceRange}
          onChange={handleChange}
          options={[
            { value: "Low", label: "$ Low" },
            { value: "Mid", label: "$$ Mid-range" },
            { value: "Luxury", label: "$$$ Luxury" },
          ]}
          errorMsg={errors.priceRange}
        />

        <h2 className={styles.subheader}>Contact & Social Media</h2>
        <Divider
          sx={{
            height: "1px",
            width: "100%",
            bgcolor: "var(--Green-Perfect)",
            marginBottom: "20px",
          }}
        />
        <div className={styles.contactButtons}>
          <button
            type="button"
            onClick={() => addContactInfo("phone")}
            className={styles.addButton}
          >
            + Add Phone Number
          </button>
          <button
            type="button"
            onClick={() => addContactInfo("website")}
            className={styles.addButton}
          >
            + Add Website
          </button>
          <button
            type="button"
            onClick={addSocialMediaLink}
            className={styles.addButton}
          >
            + Add Social Media
          </button>
        </div>
        {formData.contactInfo.map((contact, index) => (
          <DynamicInput
            key={index}
            label={contact.type === "phone" ? "Phone Number" : "Website"}
            type={contact.type === "phone" ? "tel" : "url"}
            value={contact.value}
            onChange={(e) => handleContactChange(index, e.target.value)}
            errorMsg={errors[`contactInfo-${index}`]}
            onRemove={() => removeContactInfo(index)}
          />
        ))}
        {formData.socialMediaLinks.map((link, index) => (
          <div key={index} className={styles.contactInput}>
            <Dropdown
              label="Platform"
              id={`socialMediaPlatform-${index}`}
              required
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
              required
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

        {success && <SuccessMessage message={success} />}
        {error && <ErrorMessage message={error} />}

        <button
          type="submit"
          className={styles.submitButton}
          disabled={loading}
        >
          {loading ? "Creating..." : "Create Destination"}
        </button>
      </form>
    </div>
  );
}
