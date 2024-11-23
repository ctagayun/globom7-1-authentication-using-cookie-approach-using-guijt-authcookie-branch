import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { currencyFormatter } from "../config";
import { useFetchHouses } from "../hooks/HouseHooks";
import { House } from "../types/house";
import useFetchUser from "../hooks/UserHooks";
import ApiStatus from "../apiStatus";

const HouseList = () => {
  const nav = useNavigate();
  const { data, status, isSuccess } = useFetchHouses();

  ////*Step4: Call useFetchUser hook here too
  const { data: userClaims } = useFetchUser();

  if (!isSuccess) return <ApiStatus status={status}></ApiStatus>;

  return (
    <div>
      <div className="row mb-2">
        <h5 className="themeFontColor text-center">
          Houses currently on the market
        </h5>
      </div>
      <table className="table table-hover">
        <thead>
          <tr>
            <th>Address</th>
            <th>Country</th>
            <th>Asking Price</th>
          </tr>
        </thead>
        <tbody>
          {data &&
            data.map((h: House) => (
              <tr key={h.id} onClick={() => nav(`/house/${h.id}`)}>
                <td>{h.address}</td>
                <td>{h.country}</td>
                <td>{currencyFormatter.format(h.price)}</td>
              </tr>
            ))}
        </tbody>
      </table>
      //*There'sno point showing Add button if the user is not an Admin
      {userClaims &&
        //*So look at the claims if the user has the rights. In other words a "role" with the value "admin"
        //*Amd only in this case I am displaying the Add Button
        userClaims.find((c) => c.type === "role" && c.value === "Admin") && (
          <Link className="btn btn-primary" to="/house/add">
            Add
          </Link>
        )}
    </div>
  );
};

export default HouseList;
