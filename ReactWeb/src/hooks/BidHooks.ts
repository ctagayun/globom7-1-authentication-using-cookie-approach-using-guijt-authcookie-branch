import { Bid } from "./../types/bid";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import Config from "../config";
import axios, { AxiosError, AxiosResponse } from "axios";
import Problem from "../types/problem";


//*to support cookie authentication in React application modify:
//*src/hooks/BidHooks and HouseHooks and instruct Axios to send along the cookie 
//*with each Ajax call. But in order not to repeat each insturection on 
//*every endpoint add "Axios.defaults.withCredntials" to src/main.tsx
const useFetchBids = (houseId: number) => {
  return useQuery<Bid[], AxiosError<Problem>>({
    queryKey: ["bids", houseId],
    queryFn: () =>
      axios
        .get(`${Config.baseApiUrl}/house/${houseId}/bids`)
        .then((resp) => resp.data),
  });
};

const useAddBid = () => {
  const queryClient = useQueryClient();
  return useMutation<AxiosResponse, AxiosError<Problem>, Bid>({
    mutationFn: (b) =>
      axios.post(`${Config.baseApiUrl}/house/${b.houseId}/bids`, b),
    onSuccess: (_, bid) => {
      queryClient.invalidateQueries({
        queryKey: ["bids", bid.houseId],
      });
    },
  });
};

export { useFetchBids, useAddBid };
