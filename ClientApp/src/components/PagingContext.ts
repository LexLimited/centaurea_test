import { createContext, useContext } from 'react';

export const PagingContext = createContext({
  current: 1,
  total: 1,
  size: 10,
  setCurrent: () => {},
} as {
  current: number;
  total: number;
  size: number;
  setCurrent: (val: number) => void;
});

export const usePaging = () => useContext(PagingContext)
