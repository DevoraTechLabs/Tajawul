"use client";

import styles from "@/trip.module.css";
import { useState } from "react";
import API from "@/utils/api";
import useAuthStore from "@/store/authStore"; // Add this import

export default function DeleteConfirmation({ onClose, onConfirm, tripId }) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState(null);
  const { accessToken } = useAuthStore(); // Get access token

  const handleDelete = async () => {
    setIsDeleting(true);
    setError(null);
    
    try {
      // Add authorization header
      const response = await API.delete(`/Trip/${tripId}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`
        }
      });

      // Check response structure based on your API
      if (response.status !== 200 && response.status !== 204) {
        throw new Error(response.data?.message || 'Failed to delete trip');
      }

      onConfirm(); // Only call onConfirm if deletion was successful
    } catch (err) {
      console.error("Delete error:", err);
      setError(err.response?.data?.message || err.message || 'Failed to delete trip');
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <div className={styles.popupOverlay}>
      <div className={styles.popupContainer}>
        <h2>Delete Trip</h2>
        <p>Are you sure you want to delete this trip? This action cannot be undone.</p>
        
        {error && <p className={styles.errorMessage}>{error}</p>}
        
        <div className={styles.popupButtons}>
          <button 
            className={styles.popupCancelButton}
            onClick={onClose}
            disabled={isDeleting}
          >
            Cancel
          </button>
          <button 
            className={styles.popupConfirmButton}
            onClick={handleDelete}
            disabled={isDeleting}
          >
            {isDeleting ? 'Deleting...' : 'Delete'}
          </button>
        </div>
      </div>
    </div>
  );
}