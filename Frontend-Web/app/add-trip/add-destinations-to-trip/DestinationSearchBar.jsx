// DestinationSearchBar.jsx
"use client";
import React, { useState } from "react";
import styles from "./DestinationSearchResults.module.css";
import SearchIcon from "@mui/icons-material/Search";
import FavoriteBorderIcon from "@mui/icons-material/FavoriteBorder";
import PlaceIcon from "@mui/icons-material/Place";
import { WhiteLoading } from "app/components/ui/Loading";
import Image from "next/image";
import PriceRange from "app/components/ui/tags/PriceRange";
import ErrorMessage from "@/components/ui/ErrorMessage";
import SuccessMessage from "@/components/ui/SuccessMessage";
import API from "@/utils/api";

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || '';

const DestinationSearchBar = ({ 
  tripId,
  onDestinationAdded,
  selectedDestinations = [],
  placeholder = "Search destinations..."
}) => {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [showDayPopup, setShowDayPopup] = useState(false);
  const [selectedDestination, setSelectedDestination] = useState(null);
  const [selectedDay, setSelectedDay] = useState(1);
  const [isAdding, setIsAdding] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const fallbackImage = "/fallback.jpg";

  // Auto-dismiss messages
  React.useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(""), 5000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  React.useEffect(() => {
    if (success) {
      const timer = setTimeout(() => setSuccess(""), 5000);
      return () => clearTimeout(timer);
    }
  }, [success]);

  const fetchDestinationDetails = async (destinationId) => {
    try {
      const url = `${BASE_URL}/Destination?DestinationId=${destinationId}`;
      const res = await fetch(url);
      const data = await res.json();
      return data.destinations?.[0] || {};
    } catch (error) {
      console.error("Error fetching destination details:", error);
      return {};
    }
  };

  const fetchResults = async () => {
    if (!query) {
      setResults(null);
      return;
    }

    try {
      setIsLoading(true);
      const url = `${BASE_URL}/Search/destination?query=${query}`;
      const res = await fetch(url);
      const data = await res.json();
      
      const destinationsWithDetails = await Promise.all(
        (data.destinations || []).map(async (destination) => {
          const details = await fetchDestinationDetails(destination.id);
          const location = `${details.city || ''}${details.city && details.country ? ', ' : ''}${details.country || ''}` || "Unknown location";
          return { 
            ...destination,
            coverImage: details.coverImage || fallbackImage,
            location: location,
            priceRange: details.priceRange || "N/A",
            type: details.type || "Unknown type"
          };
        })
      );
      
      setResults(destinationsWithDetails);
    } catch (error) {
      console.error("Search error:", error);
      setResults([]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleChange = (e) => {
    setQuery(e.target.value);
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') {
      fetchResults();
    }
  };

  const handleSearchClick = () => {
    fetchResults();
  };

  const handleSelect = async (destination) => {
    try {
      setIsLoading(true);
      const details = await fetchDestinationDetails(destination.id);
      const location = `${details.city || ''}${details.city && details.country ? ', ' : ''}${details.country || ''}` || "Unknown location";
      
      const updatedDestination = {
        ...destination,
        coverImage: details.coverImage || fallbackImage,
        location: location,
        priceRange: details.priceRange || "N/A",
        type: details.type || "Unknown type"
      };

      setSelectedDestination(updatedDestination);
      const maxDay = selectedDestinations.reduce((max, d) => Math.max(max, d.day || 0), 0);
      setSelectedDay(maxDay + 1);
      setShowDayPopup(true);
    } catch (error) {
      console.error("Error fetching destination details:", error);
      setSelectedDestination(destination);
      const maxDay = selectedDestinations.reduce((max, d) => Math.max(max, d.day || 0), 0);
      setSelectedDay(maxDay + 1);
      setShowDayPopup(true);
    } finally {
      setIsLoading(false);
    }
  };

  const handleAddWithDay = async () => {
  if (!selectedDestination || !tripId) return;
  
  try {
    setIsAdding(true);
    setError("");
    setSuccess("");
    
    const existing = selectedDestinations.find(d => d.destination.id === selectedDestination.id);
    if (existing) {
      setError("This destination already exists in your trip");
      setShowDayPopup(false);
      return;
    }

    const response = await API.post(`${BASE_URL}/Trip/${tripId}/destination/${selectedDestination.id}`, {
      day: selectedDay
    });

    if (response.status >= 200 && response.status < 300) {
      setSuccess("Destination added successfully!");
      onDestinationAdded();
      setShowDayPopup(false);
      setQuery("");
      setResults(null);
    } else {
      throw new Error(response.data?.message || "Failed to add destination");
    }
  } catch (err) {
    console.error("Failed to add destination:", err);
    setError(err.message || "Failed to add destination. Please try again.");
  } finally {
    setIsAdding(false);
  }
};

  const handleImageError = (e, destinationId) => {
    e.target.src = fallbackImage;
    setResults(prev => prev?.map(d => 
      d.id === destinationId ? { ...d, coverImage: fallbackImage } : d
    ));
  };

  return (
    <div className={styles.searchContainer}>
      
      <div className={styles.inputWrapper}>
        <input
          type="text"
          placeholder={placeholder}
          value={query}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          className={styles.searchInput}
        />
        <div 
          className={styles.searchIcon}
          onClick={handleSearchClick}
        >
          {isLoading ? <WhiteLoading /> : <SearchIcon />}
        </div>
      </div>

      {error && (
        <div style={{ marginTop: '1rem' }}>
          <ErrorMessage message={error} />
        </div>
      )}

      {success && (
        <div style={{ marginTop: '1rem' }}>
          <SuccessMessage message={success} />
        </div>
      )}

      {results && (
        <div className={styles.destinationCardsContainer}>
          <div className={styles.destinationCardsGrid}>
            {results.length === 0 ? (
              <div className={styles.noResults}>No destinations found</div>
            ) : (
              results.map((destination) => (
                <div 
                  key={destination.id}
                  className={styles.destinationCard}
                >
                  <div className={styles.cardImageWrapper}>
                    <Image
                      src={destination.coverImage || fallbackImage}
                      alt={destination.name}
                      width={260}
                      height={180}
                      className={styles.cardImage}
                      onError={(e) => handleImageError(e, destination.id)}
                    />
                    
                    <div className={styles.topContainer}>
                      <PriceRange priceRange={destination.priceRange} />
                    </div>
                    
                    <div className={styles.locationTag}>
                      <PlaceIcon style={{ fontSize: "16px" }} />
                      <span>{destination.location}</span>
                    </div>
                  </div>
                  
                  <div className={styles.cardContent}>
                    <h3 className={styles.cardName}>{destination.name}</h3>
                    <p className={styles.cardType}>{destination.type}</p>
                    <button
                      className={styles.addButton}
                      onClick={() => handleSelect(destination)}
                      disabled={selectedDestinations.some(d => d.destination.id === destination.id)}
                    >
                      {selectedDestinations.some(d => d.destination.id === destination.id) 
                        ? "✓ Added to Trip" 
                        : "+ Add to Trip"}
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}

      {showDayPopup && (
        <div className={styles.dayPopupOverlay}>
          <div className={styles.dayPopup}>
            <h3>Which Day for This Destination?</h3>
            
            {isLoading ? (
              <div className={styles.loadingContainer}>
                <WhiteLoading />
              </div>
            ) : (
              <>
                <div className={styles.popupImage}>
                  <Image
                    src={selectedDestination?.coverImage || fallbackImage}
                    alt={selectedDestination?.name || "Destination"}
                    width={400}
                    height={300}
                    className={styles.coverImage}
                    onError={(e) => {
                      e.target.src = fallbackImage;
                      if (selectedDestination) {
                        setSelectedDestination(prev => ({
                          ...prev,
                          coverImage: fallbackImage
                        }));
                      }
                    }}
                  />
                </div>
                
                <div className={styles.popupDestinationInfo}>
                  <h4>{selectedDestination?.name}</h4>
                </div>
                
                <div className={styles.dayInputContainer}>
                  <input
                    type="number"
                    min="1"
                    value={selectedDay}
                    onChange={(e) => setSelectedDay(Number(e.target.value))}
                    className={styles.dayInput}
                  />
                </div>
                
                <div className={styles.popupButtons}>
                  <button 
                    className={styles.popupCancelButton}
                    onClick={() => setShowDayPopup(false)}
                    disabled={isAdding}
                  >
                    Cancel
                  </button>
                  <button 
                    className={styles.popupConfirmButton}
                    onClick={handleAddWithDay}
                    disabled={isAdding}
                  >
                    {isAdding ? "Adding..." : "Add"}
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default DestinationSearchBar;