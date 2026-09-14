import React from "react";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline"; // Import the icon

const ErrorMessage = ({ message }) => {
  return (
    <div className="errorMessage">
      <ErrorOutlineIcon
        style={{
          color: "var(--Error-Text)",
          fontSize: "20px",
          marginRight: "8px",
        }}
      />
      {message}
    </div>
  );
};

export default ErrorMessage;
