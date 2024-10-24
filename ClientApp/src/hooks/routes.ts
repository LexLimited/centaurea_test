import { useEffect } from "react";
import { useNavigate, useMatches } from "react-router-dom";

export const useRefresh = () => {
  const nav = useNavigate();
  return () => nav(0);
};

export const useRouteChange = (cb: () => void) => {
  const routes = useMatches();
  useEffect(() => {
    return cb;
  }, [routes.at(-1)]);
};

export const useNativeNavigate = (s: string | Location) => {
  useEffect(() => {
    // @ts-ignore
    location = s;
  }, []);
};
