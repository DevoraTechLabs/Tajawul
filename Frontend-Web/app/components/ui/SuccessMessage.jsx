import React from "react";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";

const SuccessMessage = ({ message }) => {
  return (
    <div className="successMessage">
      <CheckCircleOutlineIcon
        style={{
          color: "var(--Success-Text)",
          fontSize: "20px",
          marginRight: "8px",
        }}
      />
      {message}
    </div>
  );
};

export default SuccessMessage;
