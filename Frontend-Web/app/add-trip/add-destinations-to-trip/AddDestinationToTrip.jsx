"use client";
import { useState, useEffect, useRef } from 'react';
import { useRouter } from 'next/navigation';
import Image from 'next/image';
import styles from "./TripCreation.module.css";
import API from "@/utils/api";
import DestinationSearchBar from "./DestinationSearchBar";
import NavBar from "@/components/ui/NavBar";
import withAuth from "@/utils/withAuth";
import SuccessMessage from "@/components/ui/SuccessMessage";
import ErrorMessage from "@/components/ui/ErrorMessage";
import HighlightOffIcon from '@mui/icons-material/HighlightOff';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || '';

function TripDestinationList({ destinations, onRemove, loading, destinationRefs, highlightedDestination, onDayChange }) {
  // Group destinations by day
  const destinationsByDay = destinations.reduce((acc, destination) => {
    if (!acc[destination.day]) {
      acc[destination.day] = [];
    }
    acc[destination.day].push(destination);
    return acc;
  }, {});

  // Sort days in ascending order
  const sortedDays = Object.keys(destinationsByDay).sort((a, b) => a - b);

  return (
    <div className={styles.destinationList}>
      {destinations.length === 0 ? (
        <p className={styles.emptyMessage}>No destinations added yet</p>
      ) : (
        <div className={styles.daysContainer}>
          {sortedDays.map(day => (
            <div key={day} className={styles.dayGroup}>
              <div className={styles.dayHeader}>Day {day}</div>
              <div className={styles.destinationsContainer}>
                {destinationsByDay[day].map(item => (
                  <div
                    key={item.id}
                    ref={el => destinationRefs.current[item.id] = el}
                    className={`${styles.destinationItem} ${
                      highlightedDestination === item.id ? styles.highlighted : ''
                    }`}
                  >
                  <div className={styles.destinationImageWrapper}>
                    <Image
                      src={item.destination.coverImage || '/default-destination.jpg'}
                      alt={item.destination.name}
                      fill
                      quality={90}
                      priority={false}
                      className={styles.destinationImage}
                      onError={(e) => {
                        e.target.onerror = null;
                        e.target.src = '/default-destination.jpg';
                      }}
                    />
                  </div>
                    <div className={styles.destinationNameContainer}>
                      <span className={styles.destinationName}>{item.destination.name}</span>
                    </div>
                    <button 
                      onClick={() => onRemove(item.id)}
                      className={styles.removeButton}
                      disabled={loading.submit || loading.removing[item.id]}
                    >
                      {loading.removing[item.id] ? (
                        'Removing...'
                      ) : (
                        <HighlightOffIcon fontSize="small" />
                      )}
                    </button>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function AddDestinationsPage() {
  const router = useRouter();
  const [tripId, setTripId] = useState('');
  const [loading, setLoading] = useState({ 
    initial: true, 
    submit: false, 
    removing: {} 
  });
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [destinations, setDestinations] = useState([]);
  const [highlightedDestination, setHighlightedDestination] = useState(null);
  const destinationRefs = useRef({});

  // Auto-dismiss messages after 5 seconds
  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(""), 5000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  useEffect(() => {
    if (success) {
      const timer = setTimeout(() => setSuccess(""), 5000);
      return () => clearTimeout(timer);
    }
  }, [success]);

  useEffect(() => {
    const storedTripId = localStorage.getItem("tripId");
    if (storedTripId) {
      setTripId(storedTripId);
      fetchExistingDestinations(storedTripId);
    } else {
      router.push('/add-trip');
    }
  }, [router]);

  const fetchExistingDestinations = async (id) => {
    try {
      setLoading(prev => ({ ...prev, initial: true }));
      const response = await API.get(`${BASE_URL}/Trip/${id}/destinations`);
      
      if (response.data.length === 0) {
        setSuccess("Start building your trip by adding destinations!");
      } else {
        setDestinations(response.data.map(item => ({
          id: item.destinationId,
          day: item.day,
          destination: {
            id: item.destinationId,
            name: item.name,
            coverImage: item.coverImage || '/default-destination.jpg'
          }
        })));
        setSuccess("");
      }
    } catch (err) {
      if (err.response?.status === 404) {
        setSuccess("Start building your trip by adding destinations!");
      } else {
        console.error("Failed to load trip data:", err);
        setError("Failed to load trip data. Please try again.");
      }
    } finally {
      setLoading(prev => ({ ...prev, initial: false }));
    }
  };

  const handleDayChange = async (id, newDay) => {
    const dayNum = Number(newDay);
    if (dayNum < 1) {
      setError("Day must be at least 1");
      return;
    }

    const conflict = destinations.find(d => d.day === dayNum && d.id !== id);
    if (conflict) {
      setError(`Day ${dayNum} already has a destination`);
      return;
    }

    const destination = destinations.find(d => d.id === id);
    if (!destination) return;

    try {
      setLoading(prev => ({ ...prev, submit: true }));
      await API.put(`${BASE_URL}/Trip/${tripId}/destination/${id}`, { day: dayNum });
      setDestinations(prev => 
        prev.map(d => d.id === id ? { ...d, day: dayNum } : d)
      );
      setSuccess("Day updated successfully!");
    } catch (err) {
      console.error("Failed to update day:", err);
      setError("Failed to update day");
    } finally {
      setLoading(prev => ({ ...prev, submit: false }));
    }
  };

  const handleRemoveDestination = async (destinationId) => {
    try {
      setLoading(prev => ({ 
        ...prev, 
        removing: { ...prev.removing, [destinationId]: true } 
      }));
      setError("");
      
      await API.delete(`${BASE_URL}/Trip/${tripId}/destination/${destinationId}`);
      
      setDestinations(prev => prev.filter(d => d.id !== destinationId));
      setSuccess("Destination removed successfully!");
    } catch (err) {
      console.error("Delete Error:", err.response?.data || err.message);
      setError(err.response?.data?.message || "Failed to remove destination");
    } finally {
      setLoading(prev => ({ 
        ...prev, 
        removing: { ...prev.removing, [destinationId]: false } 
      }));
    }
  };

 const handleSaveTrip = async () => {
  try {
    setLoading(prev => ({ ...prev, submit: true }));
    setError("");
    setSuccess("");

    // Validate we have destinations
    if (destinations.length === 0) {
      throw new Error("Please add at least one destination before saving.");
    }

 setSuccess("Trip saved successfully!");
    
    // Immediately redirect to the trip details page
    router.push(`/triphub/${tripId}`);
    
  } catch (err) {
    console.error("Save Error:", err);
    setError(err.message || "Failed to save trip");
  } finally {
    setLoading(prev => ({ ...prev, submit: false }));
  }
};

  if (loading.initial) return (
    <div className={styles.container}>
      <NavBar />
      <div className={styles.loading}>Loading trip data...</div>
    </div>
  );

  return (
    <div className={styles.container}>
      <NavBar />
      <div className={styles.destinationsPage}>
        <div className={styles.searchColumn}>
          <h2 className={styles.searchTitle}>Unlock More Adventures—Search & Add Destinations!</h2>
          
          <div className={styles.searchSection}>
            <DestinationSearchBar 
              tripId={tripId}
              onDestinationAdded={() => fetchExistingDestinations(tripId)}
              selectedDestinations={destinations}
            />
            
            {/* Error message appears only here, below search bar */}
            {error && (
              <div style={{ marginTop: '1rem' }}>
                <ErrorMessage message={error} />
              </div>
            )}
          </div>
        </div>

        <div className={styles.destinationsColumn}>
          <h2 className={styles.listTitle}>Build Your Dream Trip</h2>
          
          {/* Success message appears only here, above list */}
          {success && (
            <div style={{ marginBottom: '1rem' }}>
              <SuccessMessage message={success} />
            </div>
          )}

          <TripDestinationList 
            destinations={destinations}
            onRemove={handleRemoveDestination}
            onDayChange={handleDayChange}
            loading={loading}
            destinationRefs={destinationRefs}
            highlightedDestination={highlightedDestination}
          />

          {destinations.length > 0 && (
            <button
              onClick={handleSaveTrip}
              className={styles.submitButton}
              disabled={
                loading.submit || 
                Object.values(loading.removing).some(Boolean)
              }
            >
              {loading.submit ? "Saving..." : "Save Trip"}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

export default withAuth(AddDestinationsPage);