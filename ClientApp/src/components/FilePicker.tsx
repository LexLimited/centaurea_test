import { Button } from '@fluentui/react-components';
import { ArrowUpload16Filled } from '@fluentui/react-icons';
import { useI18n } from './I18nContext';
import { ReactNode, useState } from 'react';

export const FilePicker = ({
  children,
  uploadCallback,
}: {
  uploadCallback?: (file?: File) => void;
  children?: (file?: File) => ReactNode;
}) => {
  const { t } = useI18n();
  const [file, setFile] = useState<File>();

  return (
    <>
      <div>
        <Button
          icon={<ArrowUpload16Filled />}
          size="small"
          appearance="primary"
          onClick={e => ((e.target as HTMLButtonElement).nextElementSibling as HTMLInputElement).click()}
        >
          {t('Upper.Upload')}
        </Button>
        <input
          type="file"
          hidden
          onChange={e => {
            if (e.target.files?.length) {
              setFile(e.target.files[0]);
              uploadCallback?.(e.target.files[0]);
            }
            else setFile(undefined)
          }}
        />
      </div>
      {children?.(file)}
    </>
  );
};
