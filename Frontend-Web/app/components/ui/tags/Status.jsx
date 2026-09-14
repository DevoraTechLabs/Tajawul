import styles from "./Status.module.css";
import DoneAllIcon from '@mui/icons-material/DoneAll';
import CancelIcon from '@mui/icons-material/Cancel';
import AutorenewIcon from '@mui/icons-material/Autorenew';
import NotStartedIcon from '@mui/icons-material/NotStarted';

const Status = ({ status }) => {
  if (!status) return null;

  const getStatusDetails = (status) => {
    if (!status) return null;

    const lowerStatus = status.toLowerCase();

    switch (lowerStatus) {
      case "completed":
        return {
          text: "Completed",
          icon: <DoneAllIcon fontSize="small" />,
          className: styles.completed
        };
      case "canceled":
      case "cancelled":
        return {
          text: "Canceled",
          icon: <CancelIcon fontSize="small" />,
          className: styles.canceled
        };
      case "inprogress":
      case "in progress":
        return {
          text: "In Progress",
          icon: <AutorenewIcon fontSize="small" />,
          className: styles.inProgress
        };
      case "notstarted":
      case "not started":
        return {
          text: "Not Started",
          icon: <NotStartedIcon fontSize="small" />,
          className: styles.notStarted
        };
      default:
        return null;
    }
  };

  const details = getStatusDetails(status);
  if (!details) return null;

  return (
    <div className={styles.statusContainer}>
      <span className={`${styles.tag} ${details.className}`}>
        <span className={styles.iconTextWrapper}>
          {details.icon}
          {details.text}
        </span>
      </span>
    </div>
  );
};

export default Status;