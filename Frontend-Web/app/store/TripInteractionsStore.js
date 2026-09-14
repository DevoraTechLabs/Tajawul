import { create } from "zustand";

const useTripInteractionsStore = create((set) => ({
  favoritesCount: 0,
  wishesCount: 0,
  clonesCount: 0,
  
  setFavoritesCount: (count) => set({ favoritesCount: count }),
  setWishesCount: (count) => set({ wishesCount: count }),
  setClonesCount: (count) => set({ clonesCount: count }),

  setAllCounts: ({ favoritesCount, wishesCount, clonesCount }) => 
    set({
      favoritesCount,
      wishesCount,
      clonesCount
    }),
      initialize: (tripId) => set({ tripId }),

}));

export default useTripInteractionsStore;