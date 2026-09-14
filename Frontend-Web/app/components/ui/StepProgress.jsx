"use client";

import React from "react";
import { usePathname, useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import styles from "./StepProgress.module.css";
import TextSnippetIcon from "@mui/icons-material/TextSnippet";
import FlightIcon from "@mui/icons-material/Flight";
import CheckIcon from "@mui/icons-material/Check";

const steps = [
  {
    id: "user-info",
    label: "User Info",
    icon: <TextSnippetIcon sx={{ fontSize: 32 }} />,
  },
  {
    id: "travel-interests",
    label: "Interests",
    icon: <FlightIcon sx={{ fontSize: 32 }} />,
  },
];

const StepProgress = ({ completedSteps = [] }) => {
  const router = useRouter();
  const pathname = usePathname();
  const [currentStep, setCurrentStep] = useState("");

  useEffect(() => {
    if (pathname) {
      setCurrentStep(pathname.split("/").pop());
    }
  }, [pathname]);

  const navigateToStep = (stepId) => {
    router.push(`/complete-your-profile/${stepId}`);
  };

  return (
    <div className={styles.progressContainer}>
      {/* Render the steps (circles and lines) */}
      {steps.map((step, index) => {
        const isCompleted = completedSteps.includes(step.id);
        const isActive = currentStep === step.id;
        const isPastStep = steps.findIndex((s) => s.id === currentStep) > index;

        return (
          <React.Fragment key={step.id}>
            {index > 0 && (
              <div
                className={styles.line}
                style={{
                  backgroundColor:
                    (isPastStep || isCompleted) && currentStep === steps[1].id
                      ? "var(--Green-Hover)"
                      : "var(--Neutrals-Light-Outline)",
                }}
              />
            )}
            <div
              className={`${styles.stepWrapper} ${
                isActive ? styles.activeStepWrapper : ""
              }`}
            >
              <div
                className={`${styles.step} ${isActive ? styles.active : ""} ${
                  isCompleted ? styles.completed : ""
                }`}
                onClick={() => navigateToStep(step.id)}
              >
                {isCompleted ? <CheckIcon sx={{ fontSize: 32 }} /> : step.icon}
              </div>
            </div>
          </React.Fragment>
        );
      })}

      {/* Render the labels separately */}
      <div className={styles.labelsContainer}>
        {steps.map((step) => {
          const isActive = currentStep === step.id;
          return (
            <span
              key={step.id}
              className={`${styles.stepLabel} ${
                isActive ? styles.activeLabel : ""
              }`}
            >
              {step.label}
            </span>
          );
        })}
      </div>
    </div>
  );
};

export default StepProgress;
