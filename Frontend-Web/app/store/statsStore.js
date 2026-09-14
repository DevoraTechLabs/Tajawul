// store/statsStore.js
import { create } from "zustand";

const useStatsStore = create((set) => ({
  wishesCount: 0,
  visitorsCount: 0,
  followersCount: 0,

  setWishesCount: (count) => set({ wishesCount: count }),
  setVisitorsCount: (count) => set({ visitorsCount: count }),
  setFollowersCount: (count) => set({ followersCount: count }),

  setAllCounts: ({ wishesCount, visitorsCount, followersCount }) =>
    set({
      wishesCount,
      visitorsCount,
      followersCount,
    }),
}));

export default useStatsStore;
