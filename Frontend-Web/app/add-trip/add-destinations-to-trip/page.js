"use client";
import NavBar from "@/components/ui/NavBar";
import AddDestinationsForm from "./AddDestinationToTrip";
import styles from "./TripCreation.module.css";
import withAuth from "@/utils/withAuth";

function AddDestinationsPage() {
  return (
    <div className={styles.container}>
      <NavBar />
      <div className={styles.destinationsPage}>
        <AddDestinationsForm />
      </div>
    </div>
  );
}

export default withAuth(AddDestinationsPage);