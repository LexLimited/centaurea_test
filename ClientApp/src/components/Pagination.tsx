import { Input, Toolbar, ToolbarButton, Button, Text } from '@fluentui/react-components';
import { ArrowNext24Filled, ArrowPrevious24Filled } from '@fluentui/react-icons';
import { usePaging } from './PagingContext';

const getWindow = (current: number, size: number, totalSize: number) => {
  if (size >= totalSize) return Array.from({ length: totalSize }, (_, i) => i + 1);

  if (current + size > totalSize) return Array.from({ length: size }, (_, i) => totalSize - size + i + 1);

  const w = Array.from({ length: size }, (_, i) => current + i);
  let pad = Math.floor(size / 2);
  while (pad > 0) {
    if (w[0] === 1) break;
    for (let i = 0; i < w.length; i++) w[i]--;
    pad--;
  }
  return w;
};

export const Pagination = () => {
  const { current, size, total, setCurrent } = usePaging();

  const pageCount = Math.floor(total / size) + 1;

  const buttons = getWindow(current, 5, pageCount);

  return (
    <Toolbar size="small">
      <ToolbarButton
        icon={<ArrowPrevious24Filled />}
        disabled={current === 1}
        onClick={() => setCurrent(current - 1)}
      />
      {buttons[0] > 1 && <div className="mx-1">...</div>}
      {buttons.map(it => {
        return (
          <Button
            key={it}
            size="small"
            appearance={it === current ? 'primary' : 'subtle'}
            icon={<Text>{it}</Text>}
            onClick={() => setCurrent(it)}
          />
        );
      })}
      {buttons.at(-1)! < pageCount && <div className="mx-1">...</div>}
      <Input className="mx-1" type="number" size="small" placeholder={`${current}/${pageCount}`} style={{ width: 70 }} />
      <ToolbarButton
        icon={<ArrowNext24Filled />}
        disabled={current == pageCount}
        onClick={() => setCurrent(current + 1)}
      />
    </Toolbar>
  );
};
