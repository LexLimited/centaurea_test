import {
  DataGrid,
  DataGridHeader,
  DataGridHeaderCell,
  DataGridBody,
  DataGridRow,
  DataGridCell,
  Toolbar,
  Text,
  Input,
  Button,
  ProgressBar,
  Field,
  createTableColumn,
} from '@fluentui/react-components';
import { PagingContext } from './PagingContext';
import { Pagination } from './Pagination';
import { PropsWithChildren, useEffect, useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useI18n } from './I18nContext';

export const SearchableTable = ({
  children,
  searchableFields,
  mergeColumns,
  size,
  loader,
}: PropsWithChildren<{
  /**
   * used to generate query form
   */
  searchableFields?: string[];
  /**
   * add custom columns or replace auto columns
   */
  mergeColumns?: Columns;
  /**
   * load table data
   */
  size?: number;
  loader: (
    q: Record<string, any> & {
      page?: number;
      size?: number;
    }
  ) => Promise<any>;
}>) => {
  const { t } = useI18n()

  const [paging, setPaging] = useState({
    current: 1,
    total: 0,
    size: size ?? 25,
  });

  const { handleSubmit, reset, control } = useForm();

  const [datagridOptions, setDatagridOptions] = useState<{
    items?: any[];
    columns?: ReturnType<typeof createTableColumn<Record<string, string>>>[];
  }>({});

  const _loader = (q: Parameters<typeof loader>[0]) => {
    q = Object.fromEntries(Object.entries(q).filter(it => !!it[1]));
    setDatagridOptions({});
    loader(q).then(items => {
      let columns = Object.keys(items[0]).map(it =>
        createTableColumn<Record<string, string>>({
          columnId: it,
          renderCell(item) {
            return item[it];
          },
          renderHeaderCell() {
            return <Text weight="semibold">{it}</Text>;
          },
        })
      );
      if (mergeColumns) {
        columns = columns.filter(it => !mergeColumns.find(x => x.columnId === it.columnId)).concat(mergeColumns);
      }
      setDatagridOptions({ items, columns });
    });
  };

  useEffect(() => {
    _loader({ page: paging.current, size: paging.size });
  }, []);

  useEffect(() => {
    setPaging({...paging, size: size ?? 25})
  },[size])

  if (!datagridOptions.items && !datagridOptions.columns) {
    return (
      <div className="p-5">
        <Field label={t("Sentence.DataIsLoading")}>
          <ProgressBar />
        </Field>
      </div>
    );
  }

  if (!datagridOptions.items?.length) {
    return (
      <div className="p-5">
        <Text>{t("Sentence.NoData")}</Text>
      </div>
    );
  }

  return (
    <>
      {(children || searchableFields) && (
        <Toolbar>
          {children}
          <div className="grow"></div>
          {searchableFields && (
            <form
              className="row align-center gap-2"
              onSubmit={handleSubmit(data => _loader(Object.assign({ page: paging.current, size: paging.size }, data)))}
            >
              {searchableFields.map((it, i) => {
                return (
                  <div className="row gap-1" key={i}>
                    <Text>{it}</Text>
                    <Controller
                      name={it}
                      control={control}
                      render={({ field }) => <Input size="small" {...field} />}
                      defaultValue=""
                    ></Controller>
                  </div>
                );
              })}
              <Button size="small" appearance="primary" type="submit">
                {t("Upper.Query")}
              </Button>
              <Button
                size="small"
                onClick={e => {
                  reset();
                  _loader({ page: paging.current, size: paging.size });
                }}
              >
                {t("Upper:Reset")}
              </Button>
            </form>
          )}
        </Toolbar>
      )}
      <DataGrid selectionMode="multiselect" columns={datagridOptions.columns!} items={datagridOptions.items!}>
        <DataGridHeader>
          <DataGridRow>
            {({ renderHeaderCell }) => <DataGridHeaderCell>{renderHeaderCell()}</DataGridHeaderCell>}
          </DataGridRow>
        </DataGridHeader>
        <DataGridBody>
          {({ item }) => (
            <DataGridRow>{({ renderCell }) => <DataGridCell>{renderCell(item)}</DataGridCell>}</DataGridRow>
          )}
        </DataGridBody>
      </DataGrid>
      <PagingContext.Provider value={{ ...paging, setCurrent: val => setPaging({ ...paging, current: val }) }}>
        <Pagination />
      </PagingContext.Provider>
    </>
  );
};

type Columns = ReturnType<typeof createTableColumn<Record<string, string>>>[];
