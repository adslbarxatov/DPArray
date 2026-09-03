_en_us_

- Implemented an alternative downloading method. DPArray will automatically enable it when GitHub `raw` URLs (the user content repository) become unavailable, but `blob` pages (the main interface) remain active;
- An automatic update option is now available for all related products. This flag can be set in the deployment window (disabled by default). If set, the update is initiated when the mini launcher successfully receives the actual list of packages;
- The process termination flag has been removed from the deployment sequence. If necessary, the app will be forcibly stopped for an update, but the user will be warned about this in advance;
- Updated the basic version of `IOP`;
- Isolated a potential issue where an incorrectly loaded package list could penetrate the parsing algorithm, causing the application to crash;
- Implemented an alternative method for loading the package list. This doesn't solve the problem with downloading packages in Russia, but it does allow you to check for updates for deployed products

⁂

_ru_ru_

- Реализован альтернативный способ загрузки. DPArray автоматически включит его, когда `raw`-адреса GitHub (хранилище пользовательского контента) станут недоступны, но страницы `blob` (главный интерфейс) останутся активными;
- Теперь для всех поддерживаемых продуктов доступна опция автоматического обновления. Соответствующий флаг можно установить в окне развёртывания (по умолчанию отключён). Если он установлен, последовательность автоматического обновления запускается, когда мини-лаунчер успешно получает актуальный список пакетов;
- Флаг завершения процесса удалён из последовательности развёртывания. При необходимости приложение будет принудительно остановлено для обновления, но пользователь будет предупреждён об этом заранее;
- Обновлена базовая версия `IOP`;
- Изолировано возможное проникновение некорректно загруженного списка пакетов в алгоритм разбора с последующим падением приложения;
- Реализован альтернативный метод загрузки списка пакетов. Он не решает проблему с загрузкой самих пакетов в РФ, но позволяет узнавать о наличии обновлений установленных продуктов

⁂
