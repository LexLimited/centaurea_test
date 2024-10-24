import { Dialog, DialogBody, DialogSurface } from '@fluentui/react-components';
import { PropsWithChildren, useState } from 'react';
import { GlobalDialogContext } from './GlobalDialogContext';

export function GlobalDialogProvider(props: PropsWithChildren) {
  const [state, setState] = useState<JSX.Element>();
  const render = (state: any) => setState(state);
  const dispose = () => setState(undefined);

  const renderDialog = () => (
    <Dialog open={!!state} onOpenChange={(_, d) => d.open || dispose()}>
      <DialogSurface>
        <DialogBody>{state}</DialogBody>
      </DialogSurface>
    </Dialog>
  );

  return (
    <GlobalDialogContext.Provider value={{ state, render, dispose }}>
      {renderDialog()}
      {props.children}
    </GlobalDialogContext.Provider>
  );
}
