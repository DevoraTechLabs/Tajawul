import NavBar from "@/components/ui/NavBar";
import FloatingChatButton from "@/components/ui/FloatingChatButton"; // Import the floating chat button component

export const metadata = {
  title: "Explore Destinations",
};

export default function Layout({ children }) {
  return (
    <div>
      <NavBar /> {children}
      <FloatingChatButton />
    </div>
  );
}
