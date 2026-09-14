"use client";

import styles from "./CompleteProfile.module.css";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import useAuthStore from "@/store/authStore";
import StepProgress from "@/components/ui/StepProgress";
import Link from "next/link";
import Image from "next/image";

function CompleteYourProfile() {
  const router = useRouter();
  const { roles } = useAuthStore();

  useEffect(() => {
    // Check if user has completed interest info but not social info
    if (roles.includes("Person") && roles.includes("CompletedInterestInfo")) {
      router.push("/complete-your-profile/user-info");
    }
    // Check if user has completed social info but not interest info
    else if (
      roles.includes("Person") &&
      roles.includes("CompletedSocialInfo")
    ) {
      router.push("/complete-your-profile/travel-interests");
    }
    // If user has completed both, redirect to home or dashboard
    else if (
      roles.includes("Person") &&
      roles.includes("CompletedInterestInfo") &&
      roles.includes("CompletedSocialInfo")
    ) {
      router.push("/");
    }
  }, [roles, router]);

  // Show profile completed content if user has "User" role
  if (roles.includes("User")) {
    return (
      <div className={styles.container}>
        <div className={styles.completedProfile}>
          <Image
            src="/profile-completed.svg"
            alt="Profile completed"
            width={350}
            height={350}
            className={styles.completedImage}
          />
          <p className={styles.completedText}>
            Your profile is all set! You're ready to explore.
          </p>
          <Link href="/explore" className={styles.ctaButton}>
            Start Exploring
          </Link>
        </div>
      </div>
    );
  }

  // Only show initial content if user has only "Person" role
  if (!roles.includes("Person") || roles.length !== 1) {
    return null; // or a loading spinner
  }

  return (
    <div className={styles.container}>
      <StepProgress />
      <p className={styles.title}>Complete Your Profile</p>
      <p className={styles.introText}>
        Before we open the doors wide to your personalized journey across the
        Arab world, we'd love to get to know you a little better. This short
        form is your daewa (invitation - دعوة) to unlock the full Tajawul
        experience.
      </p>
      <p className={styles.introText}>
        The process is simple and consists of just two steps: providing your
        basic personal information and sharing your travel interests.
      </p>
      <h4 className={styles.sectionTitle}>Why this matters:</h4>
      <p className={styles.belowTitleText}>Completing your profile helps us:</p>
      <ul className={styles.benefitsList}>
        <li className={styles.benefitItem}>
          Recommend the best destinations, and trips that match your interests
        </li>
        <li className={styles.benefitItem}>
          Personalize your travel recommendations and future trip planning
        </li>
        <li className={styles.benefitItem}>
          Unlock <strong>Social Features</strong> to connect withs travelers who
          share your vibes
        </li>
        <li className={styles.benefitItem}>
          Allow you to <strong>Add New Destinations</strong> and{" "}
          <strong>Create or Publish Trips</strong>
        </li>
        <li className={styles.benefitItem}>
          Make your profile part of the <strong>Tajawul community</strong> —
          verified, trusted, and ready to explore
        </li>
      </ul>
      <Link
        href="/complete-your-profile/user-info"
        className={styles.ctaButton}
      >
        Start Now
      </Link>
    </div>
  );
}

export default CompleteYourProfile;
