import axios, { AxiosError } from "axios";
import Config from "../config";
import { Claim } from "../types/claim";
import { useQuery } from "@tanstack/react-query";

//*This hook request to the new user endpoint in the server.
//*ASP.Netcore slides the lifetime of the session cookie each 
//*time the request is done. 
//*We dont want to reset the clock each time we call this endpoint so set: slide=false to opt out
const useFetchUser = () => {
  return useQuery<Claim[], AxiosError>({ //*Returns a claims array. Claim is just a type and value property
    queryKey: ["user"],
    queryFn: () =>
      axios
        .get(`${Config.baseApiUrl}/account/getuser?slide=false`) //* request to the new user endpoint in the server
        .then((resp) => resp.data),
  });
};
export default useFetchUser;
