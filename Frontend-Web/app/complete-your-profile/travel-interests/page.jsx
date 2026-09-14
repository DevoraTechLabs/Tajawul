"use client";

import StepThreeForm from "./StepThreeForm";
import withAuth from "@/utils/withAuth"; // ✅ Import the HOC

function StepThree() {
  return (
    <div>
      <StepThreeForm />
    </div>
  );
}

export default withAuth(StepThree); // ✅ Protect the page
