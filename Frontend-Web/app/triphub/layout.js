// layout.js
import NavBar from "@/components/ui/NavBar";

export const metadata = {
  title: "TripsHub - Discover Amazing Trips",
};

export default function Layout({ children }) {
  return (
    <div>
      <NavBar />
      {children}
    </div>
  );
}