import styles from "./GroupSize.module.css";
import PersonIcon from "@mui/icons-material/Person";
import WcIcon from "@mui/icons-material/Wc";
import FamilyRestroomIcon from "@mui/icons-material/FamilyRestroom";
import GroupIcon from "@mui/icons-material/Group";
import GroupsIcon from "@mui/icons-material/Groups";

const GroupSize = ({ groupSizes = [] }) => {
  // Return null if groupSizes is null, undefined, or empty array
  if (!groupSizes || !Array.isArray(groupSizes)) return null;

  // Filter out any null/undefined values and empty strings
  const validGroupSizes = groupSizes.filter(
    (size) => size != null && size.trim() !== ""
  );

  // Return null if there are no valid group sizes
  if (validGroupSizes.length === 0) return null;

  const getGroupSizeDetails = (size) => {
    if (!size) return null;

    const lowerSize = size.toLowerCase();

    switch (lowerSize) {
      case "solo":
        return {
          text: "Solo",
          icon: <PersonIcon fontSize="small" />,
        };
      case "couple":
        return {
          text: "Couple",
          icon: <WcIcon fontSize="small" />,
        };
      case "family":
        return {
          text: "Family",
          icon: <FamilyRestroomIcon fontSize="small" />,
        };
      case "group":
        return {
          text: "Group",
          icon: <GroupIcon fontSize="small" />,
        };
      case "big-group":
      case "big group":
        return {
          text: "Big Group",
          icon: <GroupsIcon fontSize="small" />,
        };
      default:
        return null;
    }
  };

  return (
    <div className={styles.groupSizeContainer}>
      {validGroupSizes.map((size, index) => {
        const details = getGroupSizeDetails(size);
        if (!details) return null;

        return (
          <span key={index} className={styles.tag}>
            <span className={styles.iconTextWrapper}>
              {details.icon}
              {details.text}
            </span>
          </span>
        );
      })}
    </div>
  );
};

export default GroupSize;
