"use client";
import NavBar from "@/components/ui/NavBar";
import AddDestinationsForm from "@/add-trip/add-destinations-to-trip/AddDestinationToTrip";
import styles from "@/add-trip/add-destinations-to-trip/TripCreation.module.css";
import withAuth from "@/utils/withAuth";
import { use } from 'react';

function AddDestinationsPage({ params }) {
  // Unwrap the params promise
  const { tripId } = use(params);
  
  return (
    <div className={styles.container}>
      <NavBar />
      <div className={styles.destinationsPage}>
        <AddDestinationsForm tripId={tripId} />
      </div>
    </div>
  );
}

export default withAuth(AddDestinationsPage);