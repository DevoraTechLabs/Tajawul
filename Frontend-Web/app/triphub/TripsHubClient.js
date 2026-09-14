'use client';

import TripCard from "@/components/ui/TripCard";
import styles from "./TripsHub.module.css";
import { useState, useEffect } from "react";
import { GreenLoading } from "@/components/ui/Loading";

export default function TripsHubClient({ initialTrips }) {
  const [trips, setTrips] = useState(initialTrips?.trips || initialTrips || []);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  useEffect(() => {
    const fetchTrips = async () => {
      try {
        setLoading(true);
        const response = await fetch(`${baseUrl}/Trip?Visibility=Triphub`);
        if (!response.ok) {
          throw new Error(`Failed to fetch trips (Status: ${response.status})`);
        }
              const data = await response.json();
        console.log('Client-side trips data:', data); 
        setTrips(data.trips || []);
        setError(null);
      } catch (err) {
        console.error("Error fetching trips:", err);
        setError(err.message);
        setTrips([]);
      } finally {
        setLoading(false);
      }
    };

    if (trips.length === 0) {
      fetchTrips();
    }
  }, [baseUrl, trips.length]);

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Explore Trips</h1>
      
      {loading ? (
        <div className={styles.loadingContainer}>
          <GreenLoading />
        </div>
      ) : (
        <>
          {error && <p className={styles.errorMessage}>{error}</p>}
          
          <div className={styles.tripsGrid}>
            {trips.length > 0 ? (
              trips.map((trip) => (
                <TripCard 
                  key={trip.id || `trip-${Math.random().toString(36).substr(2, 9)}`} 
                  trip={trip} 
                />
              ))
            ) : (
              <div className={styles.noTrips}>
                <p>No trips available at the moment</p>
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}