import type { ManifestDashboard } from "@umbraco-cms/backoffice/extension-registry";

const dashboards: Array<ManifestDashboard> = [
  {
    type: "dashboard",
    name: "Enterspeed.Dashboardd",
    alias: "enterspeed_dashboardd",
    elementName: "enterspeed-dashboard",
    js: () => import("./dashboard.element.js"),
    weight: -10,
    meta: {
      label: "Enterspeed.Dashboard",
      pathname: "Enterspeed.Dashboard",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content",
      },
    ],
  },
];

export const manifests = [...dashboards];
