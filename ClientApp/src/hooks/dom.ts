import { useEffect } from "react";

export const useTitle = (s: string) => {
  useEffect(() => {
    document.title = s;
    return () => {
      document.title = "";
    };
  }, []);
};
