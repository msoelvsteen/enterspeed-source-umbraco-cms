import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { EnterspeedRepository } from "./repository/enterspeed.repository";
import {
  apiResponse,
  apiResponseBase,
  customSeedNodes,
  getNumberOfPendingJobsResponse,
  seedResponse,
  enterspeedUmbracoConfigurationResponse,
  enterspeedJob,
  jobIdsToDelete,
} from "./types";

export class EnterspeedContext extends UmbContextBase<EnterspeedContext> {
  protected enterspeedRepository = new EnterspeedRepository(this);

  constructor(host: UmbControllerHost) {
    super(host, ENTERSPEED_CONTEXT);
  }

  public async customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<apiResponse<seedResponse>> {
    return await this.enterspeedRepository.customSeed(customSeedNodes);
  }

  public async seed(): Promise<apiResponse<seedResponse>> {
    return await this.enterspeedRepository.seed();
  }

  public async clearJobQueue(): Promise<apiResponseBase> {
    return await this.enterspeedRepository.clearPendingJobs();
  }

  public async getNumberOfPendingJobs(): Promise<
    apiResponse<getNumberOfPendingJobsResponse>
  > {
    return await this.enterspeedRepository.getNumberOfPendingJobs();
  }

  public async getEnterspeedConfiguration(): Promise<
    apiResponse<enterspeedUmbracoConfigurationResponse>
  > {
    return await this.enterspeedRepository.getEnterspeedConfiguration();
  }

  public async getFailedJobs(): Promise<apiResponse<enterspeedJob[]>> {
    return await this.enterspeedRepository.getFailedJobs();
  }

  public async deleteSelectedFailedJobs(
    ids: jobIdsToDelete
  ): Promise<apiResponseBase> {
    return await this.enterspeedRepository.deleteSelectedFailedJobs(ids);
  }

  public async deleteFailedJobs(): Promise<apiResponseBase> {
    return await this.enterspeedRepository.deleteFailedJobs();
  }
}

export const ENTERSPEED_CONTEXT = new UmbContextToken<EnterspeedContext>(
  EnterspeedContext.name
);
