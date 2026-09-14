"use client";

import StepOneForm from "./StepOneForm";
import withAuth from "@/utils/withAuth"; // ✅ Import the HOC

function StepOne() {
  return (
    <div>
      <StepOneForm />
    </div>
  );
}

export default withAuth(StepOne); // ✅ Protect the page
