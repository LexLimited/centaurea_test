import { Link, useMatches } from 'react-router-dom';
import { JSX } from 'react';
import { Divider } from '@fluentui/react-components';

export const Breadcrumbs = ({ routerIntegration = false }: { routerIntegration: boolean }) => {
  const routes = useMatches() as PatchedRouteRecord[];

  const breadcrumbs = routes.filter(it => it.handle?.breadcrumb);

  return (
    <div>
      {breadcrumbs.map(it => {
        return (
          <>
            {routerIntegration ? <Link to={it.pathname}>{it.handle!.breadcrumb}</Link> : it.handle!.breadcrumb}
            <Divider vertical />
          </>
        );
      })}
      {breadcrumbs.at(-1)!.handle?.breadcrumb}
    </div>
  );
};

type PatchedRouteRecord = Omit<ReturnType<typeof useMatches>[number], 'handle'> & {
  handle?: {
    breadcrumb?: JSX.Element;
  };
};
