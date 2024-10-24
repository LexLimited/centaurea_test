import { createContext, useContext } from 'react';

export const GlobalDialogContext = createContext({
  state: undefined,
  render: () => {},
  dispose: () => {},
} as {
  state?: JSX.Element;
  render: (state: JSX.Element) => void;
  dispose: () => void;
});

export const useGlobalDialog = () => useContext(GlobalDialogContext);
