"use client";

import * as React from "react";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import FormControl from "@mui/material/FormControl";
import FormLabel from "@mui/material/FormLabel";
import Box from "@mui/material/Box";

const CustomRadioGroup = ({
  label,
  value,
  onValueChange,
  children,
  style,
  required,
}) => {
  return (
    <FormControl style={style}>
      {label && (
        <FormLabel
          sx={{
            textAlign: "left",
            fontFamily: "var(--font-heading), Georgia, serif",
            fontWeight: 600,
            color: "var(--Neutrals-Black-Text, #221a14)",
            "&.Mui-focused": {
              color: "var(--Neutrals-Black-Text, #221a14)",
            },
            display: "flex",
            alignItems: "center",
            gap: "4px",
          }}
        >
          {label}
          {required && (
            <Box
              component="span"
              sx={{
                color: "#ef4444",
                fontSize: "12px",
                lineHeight: 1,
              }}
            >
              *
            </Box>
          )}
        </FormLabel>
      )}
      <RadioGroup
        row
        value={value}
        onChange={(e) => onValueChange(e.target.value)}
        sx={{
          ".MuiFormControlLabel-label": {
            fontFamily: "var(--font-body), system-ui, sans-serif",
            color: "var(--Neutrals-Black-Text, #221a14)",
          },
        }}
      >
        {children}
      </RadioGroup>
    </FormControl>
  );
};

const CustomRadio = ({ value, label, disabled, style }) => {
  return (
    <FormControlLabel
      value={value}
      control={
        <Radio
          sx={{
            color: "var(--Green-Perfect)",
            "&.Mui-checked": {
              color: "var(--Green-Perfect)",
            },
            "&.Mui-disabled": {
              opacity: 0.5,
            },
          }}
          disabled={disabled}
        />
      }
      label={label}
      style={{
        ...style,
        marginRight: "16px", // Add some spacing between options
      }}
    />
  );
};

export { CustomRadioGroup as RadioGroup, CustomRadio as RadioGroupItem };
