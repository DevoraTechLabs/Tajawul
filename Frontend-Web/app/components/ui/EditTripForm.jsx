"use client";

import { useState } from "react";
import Input from "@/components/ui/Input";
import Textarea from "@/components/ui/Textarea";
import Dropdown from "@/components/ui/Dropdown";
import ErrorMessage from "@/components/ui/ErrorMessage";
import SuccessMessage from "@/components/ui/SuccessMessage";
import Divider from "@mui/material/Divider";
import { addTripSchema } from "app/add-trip/action";
import DOMPurify from "dompurify";
import API from "@/utils/api";

export default function EditTripForm({ tripData, onClose, onSave }) {
  const [formData, setFormData] = useState({
    tripId: tripData.tripId,
    title: tripData.title,
    description: tripData.description,
    priceRange: tripData.priceRange,
    tripDuration: tripData.tripDuration,
    status: tripData.status, // No default - should come from API
    visibility: tripData.visibility // No default - should come from API
  });

  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [error, setError] = useState("");

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    const inputValue = type === "checkbox" ? checked : value;

    setFormData(prev => ({
      ...prev,
      [name]: name === "description" ? DOMPurify.sanitize(value) : inputValue,
    }));

    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: "" }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSuccess("");
    setError("");

    try {
      // Validate form data
      const validation = addTripSchema.safeParse(formData);
      if (!validation.success) {
        const newErrors = validation.error.format();
        setErrors(
          Object.keys(newErrors).reduce((acc, key) => {
            acc[key] = newErrors[key]?._errors?.[0] || "";
            return acc;
          }, {})
        );
        return;
      }

      // Make PUT request with all required fields
      const response = await API.put('/Trip', {
        tripId: formData.tripId,
        title: formData.title,
        description: formData.description,
        priceRange: formData.priceRange,
        tripDuration: formData.tripDuration,
        status: formData.status,
        visibility: formData.visibility
      });
      
      setSuccess("Trip updated successfully!");
      onSave(response.data); // Return full trip data from API

    } catch (err) {
      console.error("API Error:", err);
      setError(err.response?.data?.message || "Failed to update trip. Please try again.");
    } finally {
      setLoading(false);
    }
  };
  
  // Styles remain unchanged
  const styles = {
    popupOverlay: {
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      zIndex: 9999
    },
    popupContent: {
      position: 'relative',
      background: 'white',
      padding: '32px',
      width: '85%',
      maxWidth: '800px',
      maxHeight: '85vh',
      borderRadius: '12px',
      boxShadow: '0 4px 15px rgba(0, 0, 0, 0.3)',
      overflowY: 'auto',
      zIndex: 10000
    },
    formTitle: {
      fontSize: '1.5rem',
      color: 'var(--Green-Perfect)',
      marginBottom: '20px'
    },
    form: {
      display: 'flex',
      flexDirection: 'column',
      gap: '20px',
      width: '100%'
    },
    formRow: {
      display: 'flex',
      gap: '20px',
      width: '100%',
      '@media (max-width: 768px)': {
        flexDirection: 'column',
        gap: '15px'
      }
    },
    formButtons: {
      display: 'flex',
      justifyContent: 'flex-end',
      gap: '15px',
      marginTop: '20px'
    },
    button: {
      padding: '10px 20px',
      borderRadius: '4px',
      fontWeight: 500,
      cursor: 'pointer',
      transition: 'all 0.2s',
      border: 'none',
      ':disabled': {
        opacity: '0.7',
        cursor: 'not-allowed'
      }
    },
    cancelButton: {
      background: '#f5f5f5',
      color: '#333',
      ':hover': {
        background: '#e0e0e0'
      }
    },
    saveButton: {
      background: 'var(--Green-Perfect)',
      color: 'white',
      ':hover': {
        background: 'var(--Green-Dark)'
      }
    },
    loadingText: {
      display: 'inline-block',
      padding: '0 10px'
    },
    checkboxContainer: {
      display: 'flex',
      alignItems: 'center',
      gap: '10px',
      cursor: 'pointer',
      marginBottom: '12px',
      width: '100%'
    }
  };

  return (
    <div style={styles.popupOverlay}>
      <div style={styles.popupContent}>
        <h2 style={styles.formTitle}>Edit Trip</h2>
        <Divider sx={{ height: "1px", bgcolor: "var(--Green-Perfect)", mb: "20px" }} />
        
        <form style={styles.form} onSubmit={handleSubmit}>
          <Input
            label="Trip Title *"
            name="title"
            value={formData.title}
            onChange={handleChange}
            errorMsg={errors.title}
          />
          
          <Textarea
            label="Description *"
            name="description"
            value={formData.description}
            onChange={handleChange}
            errorMsg={errors.description}
          />

          <div style={styles.formRow}>
            <Dropdown
              label="Price Range *"
              name="priceRange"
              value={formData.priceRange}
              onChange={handleChange}
              options={[
                { value: "Low", label: "$ Low" },
                { value: "Mid", label: "$$ Mid-range" },
                { value: "Luxury", label: "$$$ Luxury" }
              ]}
              errorMsg={errors.priceRange}
            />
            
            <Dropdown
              label="Trip Duration *"
              name="tripDuration"
              value={formData.tripDuration}
              onChange={handleChange}
              options={[
                { value: "Short", label: "Short (1-3 days)" },
                { value: "Mid", label: "Mid (4-7 days)" },
                { value: "Long", label: "Long (8+ days)" }
              ]}
              errorMsg={errors.tripDuration}
            />
          </div>
        <Dropdown
          label="Visibility *"
          name="visibility"
          value={formData.visibility}
          onChange={handleChange}
          options={[
            { value: "Public", label: "Public" },
            { value: "Private", label: "Private" },
            { value: "TripHub", label: "TripHub" }
          ]}
          errorMsg={errors.visibility}
        />
        <Dropdown
          label="Status *"
          name="status"
          value={formData.status}
          onChange={handleChange}
          options={[
            { value: "Notstarted", label: "Not Started" },
            { value: "Inprogress", label: "In Progress" },
            { value: "Completed", label: "Completed" },
            { value: "Canceled", label: "Canceled" }
          ]}
          errorMsg={errors.status}
        />
          {success && <SuccessMessage message={success} />}
          {error && <ErrorMessage message={error} />}

          <div style={styles.formButtons}>
            <button 
              type="button"
              style={{ ...styles.button, ...styles.cancelButton }}
              onClick={onClose}
              disabled={loading}
            >
              Cancel
            </button>
            <button 
              type="submit"
              style={{ ...styles.button, ...styles.saveButton }}
              disabled={loading}
            >
              {loading ? "Saving..." : "Save Changes"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}